using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class RolePermission : BaseEntity
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        // Navigation properties
        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}