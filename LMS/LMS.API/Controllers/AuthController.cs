using LMS.Application.Features.Auth; // Chứa DTO
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; // Dùng để hash password

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
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            // 4. Lưu vào DB
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đăng ký thành công!" });
        }

        // POST: api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // 1. Tìm user theo email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return Unauthorized("Email hoặc mật khẩu không đúng.");
            }

            // 2. Kiểm tra password (So sánh pass nhập vào với hash trong DB)
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized("Email hoặc mật khẩu không đúng.");
            }

            // 3. Tạo Token
            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                Token = token,
                User = new { user.Id, user.FullName, user.Email }
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