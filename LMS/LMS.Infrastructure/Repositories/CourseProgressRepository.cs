using System;
using System.Threading.Tasks;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories
{
    public class CourseProgressRepository : ICourseProgressRepository
    {
        private readonly LMSDbContext _context;

        public CourseProgressRepository(LMSDbContext context)
        {
            _context = context;
        }

        public async Task<CourseProgress?> GetByEnrollmentIdAsync(Guid enrollmentId)
        {
            return await _context.CourseProgresses.FirstOrDefaultAsync(cp => cp.EnrollmentId == enrollmentId);
        }

        public async Task<CourseProgress> AddAsync(CourseProgress progress)
        {
            await _context.CourseProgresses.AddAsync(progress);
            await _context.SaveChangesAsync();
            return progress;
        }

        public async Task UpdateAsync(CourseProgress progress)
        {
            _context.CourseProgresses.Update(progress);
            await _context.SaveChangesAsync();
        }
    }
}
