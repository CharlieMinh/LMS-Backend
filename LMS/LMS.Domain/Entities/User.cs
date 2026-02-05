using System;
using System.Collections.Generic;
using LMS.Domain.Common;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Represents a user in the LMS system (Student, Instructor, or Admin)
    /// </summary>
    public class User : BaseEntity<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Course> CoursesAsInstructor { get; set; } = new List<Course>();
    }
}
