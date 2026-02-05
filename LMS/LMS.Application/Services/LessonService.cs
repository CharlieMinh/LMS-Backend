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
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;

        public LessonService(ILessonRepository lessonRepository, ICourseRepository courseRepository)
        {
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResult<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId, PagedRequest request)
        {
            var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
            var query = lessons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.ToLower();
                query = query.Where(l => l.Title.ToLower().Contains(term));
            }

            // Default sort lessons by OrderIndex if sort not specified
            if (string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = query.OrderBy(l => l.OrderIndex);
            }
            else
            {
                query = query.ApplySorting(request);
            }

            var paged = query.ToPagedResult(request);
            return new PagedResult<LessonDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<LessonDto?> GetLessonByIdAsync(Guid id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            return lesson == null ? null : MapToDto(lesson);
        }

        public async Task<LessonDto> CreateLessonAsync(CreateLessonDto dto)
        {
            // Verify course exists
            var course = await _courseRepository.GetByIdAsync(dto.CourseId);
            if (course == null) throw new KeyNotFoundException("Course not found");

            var lesson = new Lesson
            {
                Title = dto.Title,
                Content = dto.Content,
                CourseId = dto.CourseId,
                VideoUrl = dto.VideoUrl,
                OrderIndex = dto.OrderIndex,
                Status = dto.Status
            };

            await _lessonRepository.AddAsync(lesson);
            return MapToDto(lesson);
        }

        public async Task UpdateLessonAsync(Guid id, UpdateLessonDto dto)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null) throw new KeyNotFoundException("Lesson not found");

            lesson.Title = dto.Title;
            lesson.Content = dto.Content;
            lesson.VideoUrl = dto.VideoUrl;
            lesson.OrderIndex = dto.OrderIndex;
            lesson.Status = dto.Status;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _lessonRepository.UpdateAsync(lesson);
        }

        public async Task DeleteLessonAsync(Guid id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null) throw new KeyNotFoundException("Lesson not found");

            await _lessonRepository.DeleteAsync(lesson);
        }

        private static LessonDto MapToDto(Lesson lesson) => new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Content = lesson.Content,
            VideoUrl = lesson.VideoUrl,
            OrderIndex = lesson.OrderIndex,
            Status = lesson.Status,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }
}
