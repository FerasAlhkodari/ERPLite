using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPLite.Core.Domain;
using ERPLite.Core.Interfaces;
using ERPLite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .ToListAsync();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            department.CreatedAt = DateTime.UtcNow;
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            var existingDepartment = await _context.Departments.FindAsync(department.Id);
            if (existingDepartment == null)
                throw new KeyNotFoundException($"Department with ID {department.Id} not found.");

            // Update only the properties that should be updated
            existingDepartment.Name = department.Name;
            existingDepartment.Code = department.Code;
            existingDepartment.ManagerId = department.ManagerId;
            existingDepartment.UpdatedAt = DateTime.UtcNow;

            _context.Departments.Update(existingDepartment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                throw new KeyNotFoundException($"Department with ID {id} not found.");

            // Check if there are employees in this department
            var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
            if (hasEmployees)
                throw new InvalidOperationException("Cannot delete department that has employees.");

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DepartmentExistsAsync(int id)
        {
            return await _context.Departments.AnyAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesInDepartmentAsync(int departmentId)
        {
            // First check if department exists
            var departmentExists = await DepartmentExistsAsync(departmentId);
            if (!departmentExists)
                throw new KeyNotFoundException($"Department with ID {departmentId} not found.");

            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.User)
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();
        }
    }
}