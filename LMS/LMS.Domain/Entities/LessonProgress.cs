using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Tracks completion of a lesson for an enrollment
    /// </summary>
    public class LessonProgress : BaseEntity<Guid>
    {
        public Guid EnrollmentId { get; set; }
        public Guid LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public Enrollment Enrollment { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
    }
}
