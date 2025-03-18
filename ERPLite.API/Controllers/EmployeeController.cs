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
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EmployeesController(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        // GET: api/Employees
        [HttpGet]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees.Select(MapToDto));
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return MapToDto(employee);
        }

        // GET: api/Employees/Department/5
        [HttpGet("Department/{departmentId}")]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartment(int departmentId)
        {
            try
            {
                var employees = await _employeeService.GetEmployeesByDepartmentAsync(departmentId);
                return Ok(employees.Select(MapToDto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/Employees
        [HttpPost]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                // Validate department exists
                if (!await _departmentService.DepartmentExistsAsync(createEmployeeDto.DepartmentId))
                {
                    return BadRequest($"Department with ID {createEmployeeDto.DepartmentId} does not exist.");
                }

                var employee = new Employee
                {
                    EmployeeId = createEmployeeDto.EmployeeId,
                    HireDate = createEmployeeDto.HireDate,
                    DepartmentId = createEmployeeDto.DepartmentId,
                    PositionId = createEmployeeDto.PositionId,
                    Salary = createEmployeeDto.Salary,
                    ContactNumber = createEmployeeDto.ContactNumber,
                    EmergencyContact = createEmployeeDto.EmergencyContact,
                    UserId = createEmployeeDto.UserId ?? 0
                };

                var createdEmployee = await _employeeService.CreateEmployeeAsync(employee);

                return CreatedAtAction(
                    nameof(GetEmployee),
                    new { id = createdEmployee.Id },
                    MapToDto(createdEmployee));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                // Ensure employee exists
                var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
                if (existingEmployee == null)
                {
                    return NotFound($"Employee with ID {id} not found.");
                }

                // Validate department exists
                if (!await _departmentService.DepartmentExistsAsync(updateEmployeeDto.DepartmentId))
                {
                    return BadRequest($"Department with ID {updateEmployeeDto.DepartmentId} does not exist.");
                }

                // Update employee properties
                existingEmployee.EmployeeId = updateEmployeeDto.EmployeeId;
                existingEmployee.DepartmentId = updateEmployeeDto.DepartmentId;
                existingEmployee.PositionId = updateEmployeeDto.PositionId;
                existingEmployee.Salary = updateEmployeeDto.Salary;
                existingEmployee.ContactNumber = updateEmployeeDto.ContactNumber;
                existingEmployee.EmergencyContact = updateEmployeeDto.EmergencyContact;

                await _employeeService.UpdateEmployeeAsync(existingEmployee);

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

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
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

        private EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                EmployeeId = employee.EmployeeId,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                PositionId = employee.PositionId,
                PositionTitle = employee.Position?.Title,
                Salary = employee.Salary,
                ContactNumber = employee.ContactNumber,
                EmergencyContact = employee.EmergencyContact,
                UserId = employee.UserId,
                FullName = employee.User != null ? $"{employee.User.Username}" : "N/A"
            };
        }
    }
}