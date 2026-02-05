using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class LessonResource : BaseEntity<Guid>
    {
        public Guid LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? ResourceType { get; set; }
        public string? Description { get; set; }

        public Lesson Lesson { get; set; } = null!;
    }
}
