using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Aggregated progress for a course enrollment
    /// </summary>
    public class CourseProgress : BaseEntity<Guid>
    {
        public Guid EnrollmentId { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public int ProgressPercent { get; set; }

        // Navigation
        public Enrollment Enrollment { get; set; } = null!;
    }
}
