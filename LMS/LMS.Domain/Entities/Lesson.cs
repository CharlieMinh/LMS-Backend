using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class Lesson : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; } // Could be URL or text
        
        // Foreign Key
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
