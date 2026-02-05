using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories
{
    public interface ILessonResourceRepository
    {
        Task<IEnumerable<LessonResource>> GetByLessonAsync(Guid lessonId);
        Task<LessonResource?> GetByIdAsync(Guid id);
        Task<LessonResource> AddAsync(LessonResource resource);
        Task UpdateAsync(LessonResource resource);
        Task DeleteAsync(LessonResource resource);
    }
}
