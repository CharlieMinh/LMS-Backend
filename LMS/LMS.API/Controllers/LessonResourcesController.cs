using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using LMS.Application.DTOs;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/v1/lessons/{lessonId}/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonResourcesController : ControllerBase
    {
        private readonly ILessonResourceService _resourceService;

        public LessonResourcesController(ILessonResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        [HttpGet]
        [Authorize(Roles = "Student,Instructor,Admin")]
        public async Task<ActionResult<PagedResult<LessonResourceDto>>> Get(Guid lessonId, [FromQuery] PagedRequest request)
        {
            try
            {
                var result = await _resourceService.GetByLessonAsync(lessonId, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<LessonResourceDto>> Create(Guid lessonId, [FromBody] CreateLessonResourceDto dto)
        {
            try
            {
                dto.LessonId = lessonId;
                var resource = await _resourceService.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { lessonId, id = resource.Id }, resource);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Update(Guid lessonId, Guid id, [FromBody] UpdateLessonResourceDto dto)
        {
            try
            {
                await _resourceService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Delete(Guid lessonId, Guid id)
        {
            try
            {
                await _resourceService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
