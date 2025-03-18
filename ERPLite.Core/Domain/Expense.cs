using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Expense : BaseEntity
    {
        public int DepartmentId { get; set; }
        public int RequesterId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Status { get; set; }  // Pending, Approved, Rejected
        public string Category { get; set; }
        public int? BudgetId { get; set; }

        // Navigation properties
        public Department Department { get; set; }
        public Employee Requester { get; set; }
        public Budget Budget { get; set; }
        public ICollection<ExpenseApproval> Approvals { get; set; }
    }
}