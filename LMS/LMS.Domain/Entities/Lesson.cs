using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Represents a lesson within a course
    /// </summary>
    public class Lesson : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int OrderIndex { get; set; }
        
        // Foreign Keys
        public int CourseId { get; set; }
        
        // Navigation Properties
        public Course Course { get; set; } = null!;
    }
}
