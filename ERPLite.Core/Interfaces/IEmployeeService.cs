using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPLite.Core.Domain;

namespace ERPLite.Core.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<bool> EmployeeExistsAsync(int id);
    }
}