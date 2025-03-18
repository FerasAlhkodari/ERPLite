using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Position : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }

        // Navigation properties
        public Department Department { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}