using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetByIdAsync(Guid id);
        Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
        Task<IEnumerable<Enrollment>> GetByStudentAsync(Guid studentId);
        Task<IEnumerable<Enrollment>> GetByCourseAsync(Guid courseId);
        Task<Enrollment> AddAsync(Enrollment enrollment);
        Task UpdateAsync(Enrollment enrollment);
    }
}
