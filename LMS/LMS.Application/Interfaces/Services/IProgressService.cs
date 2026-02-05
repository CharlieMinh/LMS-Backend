using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces.Services
{
    public interface IProgressService
    {
        Task<LessonProgressDto> CompleteLessonAsync(Guid studentId, Guid lessonId);
        Task<IEnumerable<CourseProgressDto>> GetMyCourseProgressAsync(Guid studentId);
        Task<CourseProgressDto?> GetCourseProgressAsync(Guid studentId, Guid courseId);
    }
}
