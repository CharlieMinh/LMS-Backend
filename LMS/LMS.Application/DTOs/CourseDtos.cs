using System;
using System.ComponentModel.DataAnnotations;
using LMS.Domain.Enums;

namespace LMS.Application.DTOs
{
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
