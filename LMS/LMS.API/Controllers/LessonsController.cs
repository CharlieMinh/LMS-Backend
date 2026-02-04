using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Application.DTOs;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetByCourseId(int courseId)
        {
            return Ok(await _lessonService.GetLessonsByCourseIdAsync(courseId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LessonDto>> GetById(int id)
        {
            var lesson = await _lessonService.GetLessonByIdAsync(id);
            if (lesson == null) return NotFound();
            return Ok(lesson);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<LessonDto>> Create(CreateLessonDto dto)
        {
            try 
            {
                var lesson = await _lessonService.CreateLessonAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Course not found");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Update(int id, UpdateLessonDto dto)
        {
            try
            {
                await _lessonService.UpdateLessonAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _lessonService.DeleteLessonAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
