using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // POST /api/v1/enrollments
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<EnrollmentDto>> Enroll([FromBody] CreateEnrollmentDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var enrollment = await _enrollmentService.EnrollAsync(userId.Value, dto.CourseId);
            return Ok(enrollment);
        }

        // GET /api/v1/enrollments/me
        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<PagedResult<EnrollmentDto>>> GetMyEnrollments([FromQuery] PagedRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var items = await _enrollmentService.GetMyEnrollmentsAsync(userId.Value, request);
            return Ok(items);
        }

        // GET /api/v1/courses/{courseId}/enrollments
        [HttpGet("~/api/v1/courses/{courseId}/enrollments")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<PagedResult<EnrollmentDto>>> GetByCourse(Guid courseId, [FromQuery] PagedRequest request)
        {
            var items = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId, request);
            return Ok(items);
        }

        private Guid? GetUserId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}
