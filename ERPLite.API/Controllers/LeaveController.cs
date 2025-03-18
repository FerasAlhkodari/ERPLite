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
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetLeavesByEmployee(int employeeId)
        {
            var leaves = await _leaveService.GetLeavesByEmployeeAsync(employeeId);
            return Ok(leaves);
        }

        [HttpGet("pending-approvals/{managerId}")]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<IActionResult> GetPendingApprovals(int managerId)
        {
            var leaves = await _leaveService.GetPendingApprovalsAsync(managerId);
            return Ok(leaves);
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveRequest request)
        {
            try
            {
                var leave = await _leaveService.RequestLeaveAsync(
                    request.EmployeeId,
                    request.StartDate,
                    request.EndDate,
                    request.LeaveType,
                    request.Reason);
                return Ok(leave);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("approve/{id}")]
        [Authorize(Policy = "RequireHRRole")]
        public async Task<IActionResult> ApproveLeave(int id, [FromBody] LeaveApproval approval)
        {
            try
            {
                var leave = await _leaveService.ApproveLeaveAsync(
                    id,
                    approval.ApproverId,
                    approval.Status,
                    approval.Comments);
                return Ok(leave);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class LeaveRequest
    {
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
    }

    public class LeaveApproval
    {
        public int ApproverId { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
    }
}