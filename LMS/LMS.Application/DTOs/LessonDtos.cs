using System;
using System.ComponentModel.DataAnnotations;
using LMS.Domain.Enums;

namespace LMS.Application.DTOs
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int OrderIndex { get; set; }
        public LessonStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateLessonDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int OrderIndex { get; set; } = 0;
        [Required]
        public Guid CourseId { get; set; }
        public LessonStatus Status { get; set; } = LessonStatus.Draft;
    }

    public class UpdateLessonDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int OrderIndex { get; set; }
        public LessonStatus Status { get; set; }
    }
}
