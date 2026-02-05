using LMS.Infrastructure.Persistence; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LMS.Application.Interfaces.Services; 
using LMS.Infrastructure.Identity;
using Microsoft.OpenApi.Models;
using LMS.Infrastructure;
using LMS.Application; // Import Application DI
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using BCrypt.Net;
using LMS.API.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LMS API", Version = "v1" });

    // Cấu hình để Swagger có nút nhập Token (Ổ khóa)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 2. CẤU HÌNH DATABASE (Sửa LMSContext -> LMSDbContext)
builder.Services.AddDbContext<LMSDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Infrastructure Services (bao gồm Repository)
builder.Services.AddInfrastructure();

// Đăng ký Application Services (CourseService, LessonService...)
builder.Services.AddApplication();

// --- ĐĂNG KÝ SERVICE ---
// Đăng ký JwtService (để Controller gọi được)
builder.Services.AddScoped<IJwtService, JwtService>();

// --- CẤU HÌNH JWT AUTHENTICATION ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Apply migrations & seed admin
await SeedAdminAsync(app);

// 3. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<LMSDbContext>();
    await context.Database.MigrateAsync();

    // Ensure roles exist (already seeded by migration, but safeguard)
    var roles = new[] { "Admin", "Instructor", "Student" };
    foreach (var roleName in roles)
    {
        if (!await context.Roles.AnyAsync(r => r.Name == roleName))
        {
            context.Roles.Add(new Role { Name = roleName, CreatedAt = DateTime.UtcNow });
        }
    }

    await context.SaveChangesAsync();

    var adminEmail = "admin@lms.local";
    var adminRoleId = await context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstAsync();

    if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
    {
        var adminUser = new User
        {
            FullName = "Administrator",
            Email = adminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Status = UserStatus.Active
        };

        context.Users.Add(adminUser);
        context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRoleId });
        await context.SaveChangesAsync();
    }
}