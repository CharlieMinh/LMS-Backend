using System;
using System.Collections.Generic;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Represents a lesson within a course
    /// </summary>
    public class Lesson : BaseEntity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int OrderIndex { get; set; }
        public Enums.LessonStatus Status { get; set; } = Enums.LessonStatus.Draft;

        // Foreign Keys
        public Guid CourseId { get; set; }

        // Navigation Properties
        public Course Course { get; set; } = null!;
        public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
        public Quiz? Quiz { get; set; }
        public ICollection<LessonResource> Resources { get; set; } = new List<LessonResource>();
    }
}
