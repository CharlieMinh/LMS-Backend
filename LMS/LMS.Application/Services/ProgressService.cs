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
    public class ProgressService : IProgressService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ILessonProgressRepository _lessonProgressRepository;
        private readonly ICourseProgressRepository _courseProgressRepository;

        public ProgressService(
            IEnrollmentRepository enrollmentRepository,
            ILessonRepository lessonRepository,
            ILessonProgressRepository lessonProgressRepository,
            ICourseProgressRepository courseProgressRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _lessonRepository = lessonRepository;
            _lessonProgressRepository = lessonProgressRepository;
            _courseProgressRepository = courseProgressRepository;
        }

        public async Task<LessonProgressDto> CompleteLessonAsync(Guid studentId, Guid lessonId)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId) ?? throw new KeyNotFoundException("Lesson not found");
            var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, lesson.CourseId) ?? throw new InvalidOperationException("Bạn chưa ghi danh khóa học này.");

            // Enforce lesson order: all prior lessons must be completed
            var lessons = (await _lessonRepository.GetByCourseIdAsync(lesson.CourseId)).OrderBy(l => l.OrderIndex).ToList();
            var currentIndex = lessons.FindIndex(l => l.Id == lessonId);
            if (currentIndex > 0)
            {
                var priorLessons = lessons.Take(currentIndex).Select(l => l.Id).ToHashSet();
                var priorProgress = await _lessonProgressRepository.GetByEnrollmentAsync(enrollment.Id);
                var completed = priorProgress.Where(lp => lp.IsCompleted).Select(lp => lp.LessonId).ToHashSet();
                if (!priorLessons.All(id => completed.Contains(id)))
                    throw new InvalidOperationException("Bạn cần hoàn thành bài trước.");
            }

            var lp = await _lessonProgressRepository.GetByEnrollmentAndLessonAsync(enrollment.Id, lessonId);
            if (lp == null)
            {
                lp = new LessonProgress
                {
                    EnrollmentId = enrollment.Id,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };
                await _lessonProgressRepository.AddAsync(lp);
            }
            else
            {
                if (!lp.IsCompleted)
                {
                    lp.IsCompleted = true;
                    lp.CompletedAt = DateTime.UtcNow;
                    await _lessonProgressRepository.UpdateAsync(lp);
                }
            }

            await UpdateCourseProgress(enrollment.Id, lesson.CourseId);

            return new LessonProgressDto
            {
                EnrollmentId = lp.EnrollmentId,
                LessonId = lp.LessonId,
                IsCompleted = lp.IsCompleted,
                CompletedAt = lp.CompletedAt
            };
        }

        public async Task<IEnumerable<CourseProgressDto>> GetMyCourseProgressAsync(Guid studentId)
        {
            var enrollments = await _enrollmentRepository.GetByStudentAsync(studentId);
            var result = new List<CourseProgressDto>();
            foreach (var e in enrollments)
            {
                var cp = await _courseProgressRepository.GetByEnrollmentIdAsync(e.Id);
                if (cp != null)
                {
                    result.Add(MapCourseProgress(e.CourseId, cp));
                }
            }
            return result;
        }

        public async Task<CourseProgressDto?> GetCourseProgressAsync(Guid studentId, Guid courseId)
        {
            var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, courseId);
            if (enrollment == null) return null;
            var cp = await _courseProgressRepository.GetByEnrollmentIdAsync(enrollment.Id);
            return cp == null ? null : MapCourseProgress(courseId, cp);
        }

        private async Task UpdateCourseProgress(Guid enrollmentId, Guid courseId)
        {
            var lessons = (await _lessonRepository.GetByCourseIdAsync(courseId)).ToList();
            var total = lessons.Count;
            var completed = (await _lessonProgressRepository.GetByEnrollmentAsync(enrollmentId))
                .Count(lp => lp.IsCompleted);

            var cp = await _courseProgressRepository.GetByEnrollmentIdAsync(enrollmentId);
            if (cp == null)
            {
                cp = new CourseProgress
                {
                    EnrollmentId = enrollmentId,
                    TotalLessons = total,
                    CompletedLessons = completed,
                    ProgressPercent = total == 0 ? 0 : (int)Math.Round(completed * 100.0 / total)
                };
                await _courseProgressRepository.AddAsync(cp);
            }
            else
            {
                cp.TotalLessons = total;
                cp.CompletedLessons = completed;
                cp.ProgressPercent = total == 0 ? 0 : (int)Math.Round(completed * 100.0 / total);
                await _courseProgressRepository.UpdateAsync(cp);
            }
        }

        private static CourseProgressDto MapCourseProgress(Guid courseId, CourseProgress cp) => new CourseProgressDto
        {
            EnrollmentId = cp.EnrollmentId,
            CourseId = courseId,
            CompletedLessons = cp.CompletedLessons,
            TotalLessons = cp.TotalLessons,
            ProgressPercent = cp.ProgressPercent
        };
    }
}
