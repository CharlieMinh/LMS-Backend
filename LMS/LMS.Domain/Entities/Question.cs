using System;
using System.Collections.Generic;
using LMS.Domain.Common;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Question belonging to a quiz
    /// </summary>
    public class Question : BaseEntity<Guid>
    {
        public Guid QuizId { get; set; }
        public string Content { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public int OrderIndex { get; set; }

        // Navigation
        public Quiz Quiz { get; set; } = null!;
        public ICollection<Option> Options { get; set; } = new List<Option>();
    }
}
