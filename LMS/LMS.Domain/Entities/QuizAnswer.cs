using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Answer selected for a quiz question within a result attempt
    /// </summary>
    public class QuizAnswer : BaseEntity<Guid>
    {
        public Guid QuizResultId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid SelectedOptionId { get; set; }

        // Navigation
        public QuizResult QuizResult { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public Option SelectedOption { get; set; } = null!;
    }
}
