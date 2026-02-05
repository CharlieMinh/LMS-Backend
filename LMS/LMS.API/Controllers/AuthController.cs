using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using LMS.Application.Features.Auth; 
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; 

namespace LMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LMSDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(LMSDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/v1/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // 1. Kiểm tra email tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email này đã được sử dụng.");
            }

            // 2. Hash mật khẩu
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo User mới
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            // 4. Lưu User vào DB
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 5. Assign default "Student" role by name
            var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
            if (studentRole == null)
            {
                return StatusCode(500, "Role 'Student' chưa được cấu hình.");
            }

            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = studentRole.Id
            });
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đăng ký thành công" });
        }

        // POST: api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // 1. TASK 7: Tìm user và LOAD ROLES từ database
            // Include() để eager load UserRoles và Role navigation properties
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);
                
            if (user == null)
            {
                return Unauthorized("Email hoặc mật khẩu không đúng.");
            }

            // 2. Kiểm tra password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized("Email hoặc mật khẩu không đúng.");
            }

            // 3. Extract role names từ UserRoles
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // 4. Generate tokens (access + refresh)
            var accessToken = _jwtService.GenerateToken(user, roles);
            var refreshToken = CreateRefreshToken(user.Id);
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = new 
                { 
                    user.Id, 
                    user.FullName, 
                    user.Email,
                    Roles = roles // Trả về roles để client biết
                }
            });
        }

        // POST: api/v1/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var stored = await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (stored == null || stored.RevokedAt != null || stored.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized("Refresh token không hợp lệ hoặc đã hết hạn.");
            }

            var user = stored.User;
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var accessToken = _jwtService.GenerateToken(user, roles);

            // rotate refresh token
            stored.RevokedAt = DateTime.UtcNow;
            var newRefresh = CreateRefreshToken(user.Id);
            _context.RefreshTokens.Add(newRefresh);
            await _context.SaveChangesAsync();

            return Ok(new { AccessToken = accessToken, RefreshToken = newRefresh.Token });
        }

        // POST: api/v1/auth/logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (!string.IsNullOrEmpty(request.RefreshToken))
            {
                var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId);
                if (token != null && token.RevokedAt == null)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }
            }
            else
            {
                var tokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId && rt.RevokedAt == null).ToListAsync();
                tokens.ForEach(t => t.RevokedAt = DateTime.UtcNow);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Đăng xuất thành công" });
        }

        // POST: api/v1/auth/change-password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return Unauthorized();

            var valid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
            if (!valid) return Unauthorized("Mật khẩu hiện tại không đúng.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // revoke all refresh tokens
            var tokens = await _context.RefreshTokens.Where(rt => rt.UserId == user.Id && rt.RevokedAt == null).ToListAsync();
            tokens.ForEach(t => t.RevokedAt = DateTime.UtcNow);

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Đổi mật khẩu thành công" });
        }

        // POST: api/v1/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                // tránh lộ thông tin người dùng
                return Ok(new { Message = "Nếu email tồn tại, hệ thống đã tạo mã đặt lại." });
            }

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };
            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Nếu email tồn tại, hệ thống đã tạo mã đặt lại." });
        }

        // POST: api/v1/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var token = await _context.PasswordResetTokens
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.Token == request.Token);

            if (token == null || token.UsedAt != null || token.ExpiresAt <= DateTime.UtcNow)
            {
                return BadRequest("Mã đặt lại không hợp lệ hoặc đã hết hạn.");
            }

            token.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            token.UsedAt = DateTime.UtcNow;

            // revoke refresh tokens of user
            var tokens = await _context.RefreshTokens.Where(rt => rt.UserId == token.UserId && rt.RevokedAt == null).ToListAsync();
            tokens.ForEach(t => t.RevokedAt = DateTime.UtcNow);

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Đặt lại mật khẩu thành công" });
        }

        private RefreshToken CreateRefreshToken(Guid userId)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }

        private Guid? GetUserId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    // DTO (Data Transfer Object) - Bạn có thể tách ra file riêng trong Application layer
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LogoutRequest
    {
        public string? RefreshToken { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}