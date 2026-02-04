using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces.Services
{
    public interface ILessonService
    {
        Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(int courseId);
        Task<LessonDto?> GetLessonByIdAsync(int id);
        Task<LessonDto> CreateLessonAsync(CreateLessonDto createLessonDto);
        Task UpdateLessonAsync(int id, UpdateLessonDto updateLessonDto);
        Task DeleteLessonAsync(int id);
    }
}
