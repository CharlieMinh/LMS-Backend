using System;

namespace LMS.Application.DTOs
{
    public class LessonProgressDto
    {
        public Guid LessonId { get; set; }
        public Guid EnrollmentId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CourseProgressDto
    {
        public Guid EnrollmentId { get; set; }
        public Guid CourseId { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public int ProgressPercent { get; set; }
    }
}
