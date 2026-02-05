using System;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces.Services
{
    public interface ILessonResourceService
    {
        Task<PagedResult<LessonResourceDto>> GetByLessonAsync(Guid lessonId, PagedRequest request);
        Task<LessonResourceDto> CreateAsync(CreateLessonResourceDto dto);
        Task UpdateAsync(Guid id, UpdateLessonResourceDto dto);
        Task DeleteAsync(Guid id);
    }
}
