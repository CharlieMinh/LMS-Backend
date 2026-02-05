using System;
using System.Collections.Generic;
using LMS.Domain.Common;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Represents a course in the LMS
    /// </summary>
    public class Course : BaseEntity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        // Foreign Keys
        public Guid InstructorId { get; set; }

        // Navigation Properties
        public User Instructor { get; set; } = null!;
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}
