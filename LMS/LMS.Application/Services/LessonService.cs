using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(int courseId)
        {
            var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
            return lessons.Select(l => new LessonDto
            {
                Id = l.Id,
                Title = l.Title,
                Content = l.Content,
                CourseId = l.CourseId,
                CreatedAt = l.CreatedAt
            });
        }

        public async Task<LessonDto?> GetLessonByIdAsync(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null) return null;

            return new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                CourseId = lesson.CourseId,
                CreatedAt = lesson.CreatedAt
            };
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
                CourseId = dto.CourseId
            };

            await _lessonRepository.AddAsync(lesson);

            return new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                CourseId = lesson.CourseId,
                CreatedAt = lesson.CreatedAt
            };
        }

        public async Task UpdateLessonAsync(int id, UpdateLessonDto dto)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null) throw new KeyNotFoundException("Lesson not found");

            lesson.Title = dto.Title;
            lesson.Content = dto.Content;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _lessonRepository.UpdateAsync(lesson);
        }

        public async Task DeleteLessonAsync(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null) throw new KeyNotFoundException("Lesson not found");

            await _lessonRepository.DeleteAsync(lesson);
        }
    }
}
