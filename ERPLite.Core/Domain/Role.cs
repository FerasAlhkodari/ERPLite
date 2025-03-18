using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}