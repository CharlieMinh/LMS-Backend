using System;
using System.Threading.Tasks;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories
{
    public interface ICourseProgressRepository
    {
        Task<CourseProgress?> GetByEnrollmentIdAsync(Guid enrollmentId);
        Task<CourseProgress> AddAsync(CourseProgress progress);
        Task UpdateAsync(CourseProgress progress);
    }
}
