using System;

namespace ERPLite.Core.Domain
{
    public class Leave : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } // Annual, Sick, Personal, Unpaid
        public string Status { get; set; } // Pending, Approved, Rejected, Cancelled
        public string Reason { get; set; }
        public int? ApproverId { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public Employee Approver { get; set; }
    }
}