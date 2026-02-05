using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Password reset token for forgot/reset flow
    /// </summary>
    public class PasswordResetToken : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}
