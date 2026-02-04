using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class User : BaseEntity<Guid> // Dùng Guid cho User
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string Status { get; set; } = "Active";

        // Relationships
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
