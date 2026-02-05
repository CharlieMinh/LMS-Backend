using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories
{
    public interface ILessonRepository
    {
        Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId);
        Task<Lesson?> GetByIdAsync(Guid id);
        Task<Lesson> AddAsync(Lesson lesson);
        Task UpdateAsync(Lesson lesson);
        Task DeleteAsync(Lesson lesson);
    }
}
