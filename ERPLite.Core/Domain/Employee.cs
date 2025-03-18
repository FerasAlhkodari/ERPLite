using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeId { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public decimal Salary { get; set; }
        public string ContactNumber { get; set; }
        public string EmergencyContact { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public Department Department { get; set; }
        public Position Position { get; set; }
        public User User { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
        public ICollection<Leave> Leaves { get; set; }
    }
}