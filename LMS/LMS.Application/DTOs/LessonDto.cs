using System;

namespace LMS.Application.DTOs
{
    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
