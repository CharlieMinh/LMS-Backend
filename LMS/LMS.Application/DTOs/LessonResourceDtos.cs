using System;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.DTOs
{
    public class LessonResourceDto
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? ResourceType { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateLessonResourceDto
    {
        [Required]
        public Guid LessonId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;
        public string? ResourceType { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateLessonResourceDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;
        public string? ResourceType { get; set; }
        public string? Description { get; set; }
    }
}
