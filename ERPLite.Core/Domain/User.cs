using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public string PreferredLanguage { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}