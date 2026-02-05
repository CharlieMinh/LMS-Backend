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
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly LMSDbContext _context;

        public EnrollmentRepository(LMSDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetByIdAsync(Guid id)
        {
            return await _context.Enrollments.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
        {
            return await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentAsync(Guid studentId)
        {
            return await _context.Enrollments.Where(e => e.StudentId == studentId).ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByCourseAsync(Guid courseId)
        {
            return await _context.Enrollments.Where(e => e.CourseId == courseId).ToListAsync();
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}
