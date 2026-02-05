using System;
using System.Collections.Generic;
using LMS.Domain.Enums;

namespace LMS.Application.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public CourseStatus Status { get; set; }
        public Guid InstructorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
