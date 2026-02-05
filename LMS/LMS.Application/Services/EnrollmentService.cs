using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Application.Common;
using LMS.Application.DTOs;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseProgressRepository _courseProgressRepository;
        private readonly ILessonRepository _lessonRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository,
            ICourseProgressRepository courseProgressRepository,
            ILessonRepository lessonRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _courseProgressRepository = courseProgressRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task<EnrollmentDto> EnrollAsync(Guid studentId, Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId) ?? throw new KeyNotFoundException("Course not found");
            if (course.Status != CourseStatus.Published)
                throw new InvalidOperationException("Course is not published.");

            var existing = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, courseId);
            if (existing != null)
                throw new InvalidOperationException("Bạn đã đăng ký khóa học này.");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = EnrollmentStatus.Active,
                EnrolledAt = DateTime.UtcNow
            };

            await _enrollmentRepository.AddAsync(enrollment);

            var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
            var totalLessons = lessons.Count();
            var courseProgress = new CourseProgress
            {
                EnrollmentId = enrollment.Id,
                CompletedLessons = 0,
                TotalLessons = totalLessons,
                ProgressPercent = 0
            };
            await _courseProgressRepository.AddAsync(courseProgress);

            return MapEnrollment(enrollment);
        }

        public async Task<PagedResult<EnrollmentDto>> GetMyEnrollmentsAsync(Guid studentId, PagedRequest request)
        {
            var enrollments = await _enrollmentRepository.GetByStudentAsync(studentId);
            var query = enrollments.AsQueryable();
            query = query.ApplySorting(request);
            var paged = query.ToPagedResult(request);

            return new PagedResult<EnrollmentDto>
            {
                Items = paged.Items.Select(MapEnrollment).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<PagedResult<EnrollmentDto>> GetEnrollmentsByCourseAsync(Guid courseId, PagedRequest request)
        {
            var enrollments = await _enrollmentRepository.GetByCourseAsync(courseId);
            var query = enrollments.AsQueryable();
            query = query.ApplySorting(request);
            var paged = query.ToPagedResult(request);

            return new PagedResult<EnrollmentDto>
            {
                Items = paged.Items.Select(MapEnrollment).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        private static EnrollmentDto MapEnrollment(Enrollment e) => new EnrollmentDto
        {
            Id = e.Id,
            CourseId = e.CourseId,
            StudentId = e.StudentId,
            Status = e.Status,
            EnrolledAt = e.EnrolledAt
        };
    }
}
