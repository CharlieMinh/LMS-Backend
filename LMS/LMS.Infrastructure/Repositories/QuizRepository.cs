using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly LMSDbContext _context;

        public QuizRepository(LMSDbContext context)
        {
            _context = context;
        }

        public async Task<Quiz?> GetByIdAsync(Guid id, bool includeDetails = false)
        {
            IQueryable<Quiz> query = _context.Quizzes;
            if (includeDetails)
            {
                query = query
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.Options);
            }
            return await query.FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Quiz>> GetByCourseAsync(Guid courseId, bool includeDetails = false)
        {
            IQueryable<Quiz> query = _context.Quizzes.Where(q => q.CourseId == courseId);
            if (includeDetails)
            {
                query = query
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.Options);
            }
            return await query.ToListAsync();
        }

        public async Task<Quiz> AddAsync(Quiz quiz)
        {
            await _context.Quizzes.AddAsync(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task UpdateAsync(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Quiz quiz)
        {
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task<Question?> GetQuestionByIdAsync(Guid questionId, bool includeOptions = false)
        {
            IQueryable<Question> query = _context.Questions;
            if (includeOptions)
            {
                query = query.Include(q => q.Options);
            }
            return await query.FirstOrDefaultAsync(q => q.Id == questionId);
        }

        public async Task AddQuestionAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(Question question)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task ReplaceQuestionOptionsAsync(Question question, IEnumerable<Option> newOptions)
        {
            var existingOptions = _context.Options.Where(o => o.QuestionId == question.Id);
            _context.Options.RemoveRange(existingOptions);
            await _context.Options.AddRangeAsync(newOptions);
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task<Option?> GetOptionByIdAsync(Guid optionId)
        {
            return await _context.Options.FirstOrDefaultAsync(o => o.Id == optionId);
        }

        public async Task AddResultAsync(QuizResult result)
        {
            await _context.QuizResults.AddAsync(result);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuizResult>> GetResultsForStudentAsync(Guid quizId, Guid studentId)
        {
            return await _context.QuizResults
                .Where(r => r.QuizId == quizId && r.StudentId == studentId)
                .ToListAsync();
        }

        public async Task AddAnswersAsync(IEnumerable<QuizAnswer> answers)
        {
            await _context.QuizAnswers.AddRangeAsync(answers);
            await _context.SaveChangesAsync();
        }
    }
}
