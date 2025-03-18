using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ManagerId { get; set; }

        // Navigation properties
        public Employee Manager { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}