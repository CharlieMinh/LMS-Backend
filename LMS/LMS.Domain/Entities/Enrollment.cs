using System;
using System.Collections.Generic;
using LMS.Domain.Common;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Represents a student's enrollment in a course
    /// </summary>
    public class Enrollment : BaseEntity<Guid>
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public CourseProgress CourseProgress { get; set; } = null!;
        public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
    }
}
