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
    public class LessonResourceRepository : ILessonResourceRepository
    {
        private readonly LMSDbContext _context;

        public LessonResourceRepository(LMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LessonResource>> GetByLessonAsync(Guid lessonId)
        {
            return await _context.LessonResources
                .Where(r => r.LessonId == lessonId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<LessonResource?> GetByIdAsync(Guid id)
        {
            return await _context.LessonResources.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<LessonResource> AddAsync(LessonResource resource)
        {
            await _context.LessonResources.AddAsync(resource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task UpdateAsync(LessonResource resource)
        {
            _context.LessonResources.Update(resource);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(LessonResource resource)
        {
            _context.LessonResources.Remove(resource);
            await _context.SaveChangesAsync();
        }
    }
}
