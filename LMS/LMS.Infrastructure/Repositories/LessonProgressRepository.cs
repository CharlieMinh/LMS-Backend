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
    public class LessonProgressRepository : ILessonProgressRepository
    {
        private readonly LMSDbContext _context;

        public LessonProgressRepository(LMSDbContext context)
        {
            _context = context;
        }

        public async Task<LessonProgress?> GetByEnrollmentAndLessonAsync(Guid enrollmentId, Guid lessonId)
        {
            return await _context.LessonProgresses.FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollmentId && lp.LessonId == lessonId);
        }

        public async Task<IEnumerable<LessonProgress>> GetByEnrollmentAsync(Guid enrollmentId)
        {
            return await _context.LessonProgresses.Where(lp => lp.EnrollmentId == enrollmentId).ToListAsync();
        }

        public async Task<LessonProgress> AddAsync(LessonProgress progress)
        {
            await _context.LessonProgresses.AddAsync(progress);
            await _context.SaveChangesAsync();
            return progress;
        }

        public async Task UpdateAsync(LessonProgress progress)
        {
            _context.LessonProgresses.Update(progress);
            await _context.SaveChangesAsync();
        }
    }
}
