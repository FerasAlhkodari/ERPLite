using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPLite.Core.Domain;
using ERPLite.Core.DTOs;
using ERPLite.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;

        public DepartmentsController(IDepartmentService departmentService, IEmployeeService employeeService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments.Select(MapToDto));
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return MapToDto(department);
        }

        // GET: api/Departments/5/Employees
        [HttpGet("{id}/Employees")]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetDepartmentEmployees(int id)
        {
            try
            {
                var employees = await _departmentService.GetEmployeesInDepartmentAsync(id);
                return Ok(employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department?.Name,
                    PositionId = e.PositionId,
                    PositionTitle = e.Position?.Title,
                    Salary = e.Salary,
                    ContactNumber = e.ContactNumber,
                    EmergencyContact = e.EmergencyContact,
                    UserId = e.UserId,
                    FullName = e.User != null ? e.User.Username : "N/A"
                }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/Departments
        [HttpPost]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto createDepartmentDto)
        {
            try
            {
                // Validate manager exists if provided
                if (createDepartmentDto.ManagerId.HasValue &&
                    !await _employeeService.EmployeeExistsAsync(createDepartmentDto.ManagerId.Value))
                {
                    return BadRequest($"Employee with ID {createDepartmentDto.ManagerId.Value} does not exist.");
                }

                var department = new Department
                {
                    Name = createDepartmentDto.Name,
                    Code = createDepartmentDto.Code,
                    ManagerId = createDepartmentDto.ManagerId
                };

                var createdDepartment = await _departmentService.CreateDepartmentAsync(department);

                return CreatedAtAction(
                    nameof(GetDepartment),
                    new { id = createdDepartment.Id },
                    MapToDto(createdDepartment));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Departments/5
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto updateDepartmentDto)
        {
            try
            {
                // Ensure department exists
                var existingDepartment = await _departmentService.GetDepartmentByIdAsync(id);
                if (existingDepartment == null)
                {
                    return NotFound($"Department with ID {id} not found.");
                }

                // Validate manager exists if provided
                if (updateDepartmentDto.ManagerId.HasValue &&
                    !await _employeeService.EmployeeExistsAsync(updateDepartmentDto.ManagerId.Value))
                {
                    return BadRequest($"Employee with ID {updateDepartmentDto.ManagerId.Value} does not exist.");
                }

                // Update department properties
                existingDepartment.Name = updateDepartmentDto.Name;
                existingDepartment.Code = updateDepartmentDto.Code;
                existingDepartment.ManagerId = updateDepartmentDto.ManagerId;

                await _departmentService.UpdateDepartmentAsync(existingDepartment);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                await _departmentService.DeleteDepartmentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                ManagerId = department.ManagerId,
                ManagerName = department.Manager?.User?.Username ?? null
            };
        }
    }
}