using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Application.Common;
using LMS.Application.DTOs;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;

namespace LMS.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<PagedResult<CourseDto>> GetCoursesAsync(PagedRequest request)
        {
            var courses = await _courseRepository.GetAllAsync();
            var query = courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.ToLower();
                query = query.Where(c => (c.Title != null && c.Title.ToLower().Contains(term)) || (c.Description != null && c.Description.ToLower().Contains(term)));
            }

            var sorted = query.ApplySorting(request);
            var paged = sorted.ToPagedResult(request);

            return new PagedResult<CourseDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<CourseDto?> GetCourseByIdAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            return course == null ? null : MapToDto(course);
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                ThumbnailUrl = dto.ThumbnailUrl,
                Status = dto.Status,
                InstructorId = dto.InstructorId
            };

            await _courseRepository.AddAsync(course);
            return MapToDto(course);
        }

        public async Task UpdateCourseAsync(Guid id, UpdateCourseDto dto)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) throw new KeyNotFoundException("Course not found");

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.ThumbnailUrl = dto.ThumbnailUrl;
            course.Status = dto.Status;
            course.UpdatedAt = DateTime.UtcNow;

            await _courseRepository.UpdateAsync(course);
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) throw new KeyNotFoundException("Course not found");

            await _courseRepository.DeleteAsync(course);
        }

        private static CourseDto MapToDto(Course course) => new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            ThumbnailUrl = course.ThumbnailUrl,
            Status = course.Status,
            InstructorId = course.InstructorId,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }
}
