using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Budget : BaseEntity
    {
        public int DepartmentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public Department Department { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}