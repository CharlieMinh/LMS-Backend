using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories
{
    public interface IQuizRepository
    {
        Task<Quiz?> GetByIdAsync(Guid id, bool includeDetails = false);
        Task<IEnumerable<Quiz>> GetByCourseAsync(Guid courseId, bool includeDetails = false);
        Task<Quiz> AddAsync(Quiz quiz);
        Task UpdateAsync(Quiz quiz);
        Task DeleteAsync(Quiz quiz);

        Task<Question?> GetQuestionByIdAsync(Guid questionId, bool includeOptions = false);
        Task AddQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(Question question);

        Task ReplaceQuestionOptionsAsync(Question question, IEnumerable<Option> newOptions);

        Task<Option?> GetOptionByIdAsync(Guid optionId);

        Task AddResultAsync(QuizResult result);
        Task<IEnumerable<QuizResult>> GetResultsForStudentAsync(Guid quizId, Guid studentId);

        Task AddAnswersAsync(IEnumerable<QuizAnswer> answers);
    }
}
