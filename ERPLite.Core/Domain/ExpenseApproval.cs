using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class ExpenseApproval : BaseEntity
    {
        public int ExpenseId { get; set; }
        public int ApproverId { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string Status { get; set; }  // Approved, Rejected
        public string Comments { get; set; }

        // Navigation properties
        public Expense Expense { get; set; }
        public Employee Approver { get; set; }
    }
}