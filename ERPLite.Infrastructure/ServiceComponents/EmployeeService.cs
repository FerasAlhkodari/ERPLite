using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPLite.Core.Domain;
using ERPLite.Core.Interfaces;
using ERPLite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Infrastructure.ServiceComponents
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.User)
                .ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            employee.CreatedAt = DateTime.UtcNow;
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.Id);
            if (existingEmployee == null)
                throw new KeyNotFoundException($"Employee with ID {employee.Id} not found.");

            // Update only the properties that should be updated
            existingEmployee.EmployeeId = employee.EmployeeId;
            existingEmployee.DepartmentId = employee.DepartmentId;
            existingEmployee.PositionId = employee.PositionId;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.ContactNumber = employee.ContactNumber;
            existingEmployee.EmergencyContact = employee.EmergencyContact;
            existingEmployee.UpdatedAt = DateTime.UtcNow;

            _context.Employees.Update(existingEmployee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found.");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.User)
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }
    }
}