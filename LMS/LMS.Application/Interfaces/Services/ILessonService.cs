using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces.Services
{
    public interface ILessonService
    {
        Task<PagedResult<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId, PagedRequest request);
        Task<LessonDto?> GetLessonByIdAsync(Guid id);
        Task<LessonDto> CreateLessonAsync(CreateLessonDto createLessonDto);
        Task UpdateLessonAsync(Guid id, UpdateLessonDto updateLessonDto);
        Task DeleteLessonAsync(Guid id);
    }
}
