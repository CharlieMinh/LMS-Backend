using System;
using System.ComponentModel.DataAnnotations;
using LMS.Domain.Enums;

namespace LMS.Application.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public CourseStatus Status { get; set; }
        public Guid InstructorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCourseDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        [Required]
        public Guid InstructorId { get; set; }
    }

    public class UpdateCourseDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public CourseStatus Status { get; set; }
    }
}
