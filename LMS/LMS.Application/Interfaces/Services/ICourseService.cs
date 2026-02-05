using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces.Services
{
    public interface ICourseService
    {
        Task<PagedResult<CourseDto>> GetCoursesAsync(PagedRequest request);
        Task<CourseDto?> GetCourseByIdAsync(Guid id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
        Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto);
        Task DeleteCourseAsync(Guid id);
    }
}
