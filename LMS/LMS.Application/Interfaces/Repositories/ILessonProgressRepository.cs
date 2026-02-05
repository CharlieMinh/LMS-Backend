using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories
{
    public interface ILessonProgressRepository
    {
        Task<LessonProgress?> GetByEnrollmentAndLessonAsync(Guid enrollmentId, Guid lessonId);
        Task<IEnumerable<LessonProgress>> GetByEnrollmentAsync(Guid enrollmentId);
        Task<LessonProgress> AddAsync(LessonProgress progress);
        Task UpdateAsync(LessonProgress progress);
    }
}
