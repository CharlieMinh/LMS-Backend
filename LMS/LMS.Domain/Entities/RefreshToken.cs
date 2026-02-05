using System;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Refresh token for JWT rotation
    /// </summary>
    public class RefreshToken : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}
