using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class Role : BaseEntity<int> // Dùng int cho Role (Admin, Instructor, Student)
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
