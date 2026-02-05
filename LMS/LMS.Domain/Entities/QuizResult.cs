using System;
using System.Collections.Generic;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Result of a quiz attempt
    /// </summary>
    public class QuizResult : BaseEntity<Guid>
    {
        public Guid QuizId { get; set; }
        public Guid StudentId { get; set; }
        public int Score { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Quiz Quiz { get; set; } = null!;
        public User Student { get; set; } = null!;
        public Enrollment? Enrollment { get; set; }
        public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
    }
}
