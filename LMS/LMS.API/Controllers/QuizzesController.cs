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
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizzesController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetById(Guid id)
        {
            var quiz = await _quizService.GetQuizAsync(id);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        [HttpGet("~/api/v1/courses/{courseId}/quizzes")]
        public async Task<ActionResult<PagedResult<QuizDto>>> GetByCourse(Guid courseId, [FromQuery] PagedRequest request)
        {
            var quizzes = await _quizService.GetQuizzesByCourseAsync(courseId, request);
            return Ok(quizzes);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<QuizDto>> Create([FromBody] CreateQuizDto dto)
        {
            var quiz = await _quizService.CreateQuizAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuizDto dto)
        {
            try
            {
                await _quizService.UpdateQuizAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _quizService.DeleteQuizAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{quizId}/questions")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<QuestionDto>> AddQuestion(Guid quizId, [FromBody] CreateQuestionDto dto)
        {
            try
            {
                var question = await _quizService.AddQuestionAsync(quizId, dto);
                return Ok(question);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{quizId}/questions/{questionId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> UpdateQuestion(Guid quizId, Guid questionId, [FromBody] UpdateQuestionDto dto)
        {
            try
            {
                await _quizService.UpdateQuestionAsync(quizId, questionId, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{quizId}/questions/{questionId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> DeleteQuestion(Guid quizId, Guid questionId)
        {
            try
            {
                await _quizService.DeleteQuestionAsync(quizId, questionId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{quizId}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<QuizResultDto>> Submit(Guid quizId, [FromBody] SubmitQuizDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var result = await _quizService.SubmitAsync(quizId, userId.Value, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private Guid? GetUserId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}
