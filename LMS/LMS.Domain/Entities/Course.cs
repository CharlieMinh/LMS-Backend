using System;
using System.Collections.Generic;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class Course : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        
        // Navigation property
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
