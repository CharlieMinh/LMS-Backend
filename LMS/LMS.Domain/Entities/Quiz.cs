using System;
using System.Collections.Generic;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Quiz attached to a lesson
    /// </summary>
    public class Quiz : BaseEntity<Guid>
    {
        public Guid LessonId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? TimeLimit { get; set; }
        public int TotalScore { get; set; }

        // Navigation
        public Lesson Lesson { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizResult> Results { get; set; } = new List<QuizResult>();
    }
}
