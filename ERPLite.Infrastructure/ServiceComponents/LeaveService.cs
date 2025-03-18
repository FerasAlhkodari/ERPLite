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
    public class LeaveService : ILeaveService
    {
        private readonly ApplicationDbContext _context;

        public LeaveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Leave>> GetLeavesByEmployeeAsync(int employeeId)
        {
            return await _context.Leaves
                .Where(l => l.EmployeeId == employeeId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Leave>> GetPendingApprovalsAsync(int managerId)
        {
            // Get the departments managed by this manager
            var departmentIds = await _context.Departments
                .Where(d => d.ManagerId == managerId)
                .Select(d => d.Id)
                .ToListAsync();

            // Get leaves from employees in those departments that are pending approval
            return await _context.Leaves
                .Include(l => l.Employee)
                .Where(l => departmentIds.Contains(l.Employee.DepartmentId) &&
                           l.Status == "Pending")
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<Leave> RequestLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, string leaveType, string reason)
        {
            // Validate dates
            if (startDate > endDate)
                throw new InvalidOperationException("Start date must be before end date");

            var leave = new Leave
            {
                EmployeeId = employeeId,
                StartDate = startDate,
                EndDate = endDate,
                LeaveType = leaveType,
                Status = "Pending",
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();

            return leave;
        }

        public async Task<Leave> ApproveLeaveAsync(int id, int approverId, string status, string comments)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
                throw new KeyNotFoundException("Leave not found");

            // Only allow approval if leave is pending
            if (leave.Status != "Pending")
                throw new InvalidOperationException("Only pending leave can be approved");

            leave.Status = status;
            leave.ApproverId = approverId;
            leave.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(comments))
            {
                leave.Reason = leave.Reason + $" | Approver: {comments}";
            }

            _context.Leaves.Update(leave);
            await _context.SaveChangesAsync();

            return leave;
        }
    }
}