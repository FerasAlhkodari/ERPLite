using System;
using System.Threading.Tasks;
using ERPLite.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetAttendanceByEmployee(int employeeId)
        {
            var attendances = await _attendanceService.GetAttendanceByEmployeeAsync(employeeId);
            return Ok(attendances);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            try
            {
                var attendance = await _attendanceService.CheckInAsync(request.EmployeeId, request.Notes);
                return Ok(attendance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("check-out/{attendanceId}")]
        public async Task<IActionResult> CheckOut(int attendanceId)
        {
            try
            {
                var attendance = await _attendanceService.CheckOutAsync(attendanceId);
                return Ok(attendance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CheckInRequest
    {
        public int EmployeeId { get; set; }
        public string Notes { get; set; }
    }
}