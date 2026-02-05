using LMS.Application.Features.Auth; 
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Persistence;
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

            // 5. TASK 6: Assign default "Student" role (id=3)
            // Mọi user đăng ký mới đều là Student
            var studentRoleId = 3;
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = studentRoleId
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đăng ký thành công! Bạn đã được gán vai trò Student." });
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

            // 4. TASK 7: Generate token với roles
            var token = _jwtService.GenerateToken(user, roles);

            return Ok(new
            {
                Token = token,
                User = new 
                { 
                    user.Id, 
                    user.FullName, 
                    user.Email,
                    Roles = roles // Trả về roles để client biết
                }
            });
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
}