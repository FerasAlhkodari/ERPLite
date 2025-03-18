using System.Collections.Generic;
using System.Threading.Tasks;
using ERPLite.Core.Domain;

namespace ERPLite.Core.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(int id);
        Task<Department> CreateDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(int id);
        Task<bool> DepartmentExistsAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesInDepartmentAsync(int departmentId);
    }
}