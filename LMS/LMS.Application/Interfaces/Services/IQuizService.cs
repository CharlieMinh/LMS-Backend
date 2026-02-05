using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces.Services
{
    public interface IQuizService
    {
        Task<QuizDto?> GetQuizAsync(Guid id);
        Task<PagedResult<QuizDto>> GetQuizzesByCourseAsync(Guid courseId, PagedRequest request);
        Task<QuizDto> CreateQuizAsync(CreateQuizDto dto);
        Task UpdateQuizAsync(Guid id, UpdateQuizDto dto);
        Task DeleteQuizAsync(Guid id);

        Task<QuestionDto> AddQuestionAsync(Guid quizId, CreateQuestionDto dto);
        Task UpdateQuestionAsync(Guid quizId, Guid questionId, UpdateQuestionDto dto);
        Task DeleteQuestionAsync(Guid quizId, Guid questionId);

        Task<QuizResultDto> SubmitAsync(Guid quizId, Guid studentId, SubmitQuizDto dto);
    }
}
