using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPLite.Core.Domain;

namespace ERPLite.Core.Interfaces
{
    public interface ILeaveService
    {
        Task<IEnumerable<Leave>> GetLeavesByEmployeeAsync(int employeeId);
        Task<IEnumerable<Leave>> GetPendingApprovalsAsync(int managerId);
        Task<Leave> RequestLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, string leaveType, string reason);
        Task<Leave> ApproveLeaveAsync(int id, int approverId, string status, string comments);
    }
}