using System.Collections.Generic;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Services
{
    public interface IJwtService
    {
        /// <summary>
        /// Generate JWT token with user info and role claims
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="roles">List of role names (e.g., ["Student", "Instructor"])</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(User user, IEnumerable<string> roles);
    }
}
