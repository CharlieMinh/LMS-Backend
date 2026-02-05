using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LMS.Application.DTOs;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        // PATCH /api/v1/progress/{lessonId}
        [HttpPatch("{lessonId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<LessonProgressDto>> CompleteLesson(Guid lessonId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var progress = await _progressService.CompleteLessonAsync(userId.Value, lessonId);
            return Ok(progress);
        }

        // GET /api/v1/progress/me
        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> GetMyProgress()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var items = await _progressService.GetMyCourseProgressAsync(userId.Value);
            return Ok(items);
        }

        // GET /api/v1/courses/{courseId}/progress
        [HttpGet("~/api/v1/courses/{courseId}/progress")]
        [Authorize(Roles = "Student,Instructor,Admin")]
        public async Task<ActionResult<CourseProgressDto>> GetCourseProgress(Guid courseId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var cp = await _progressService.GetCourseProgressAsync(userId.Value, courseId);
            if (cp == null) return NotFound();
            return Ok(cp);
        }

        private Guid? GetUserId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}
