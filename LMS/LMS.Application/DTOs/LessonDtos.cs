using System.ComponentModel.DataAnnotations;

namespace LMS.Application.DTOs
{
    public class CreateLessonDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        [Required]
        public int CourseId { get; set; }
    }

    public class UpdateLessonDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
    }
}
