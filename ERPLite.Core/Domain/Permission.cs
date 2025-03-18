using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public string Resource { get; set; }
        public string Action { get; set; }

        // Navigation properties
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}