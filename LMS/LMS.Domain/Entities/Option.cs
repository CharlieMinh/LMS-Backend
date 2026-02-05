using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Option/answer choice for a question
    /// </summary>
    public class Option : BaseEntity<Guid>
    {
        public Guid QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

        // Navigation
        public Question Question { get; set; } = null!;
    }
}
