using System;

namespace ERPLite.Core.Domain
{
    public class Attendance : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; } // Present, Late, Absent
        public string Notes { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
    }
}