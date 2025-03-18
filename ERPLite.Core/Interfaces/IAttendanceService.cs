using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPLite.Core.Domain;

namespace ERPLite.Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetAttendanceByEmployeeAsync(int employeeId);
        Task<Attendance> CheckInAsync(int employeeId, string notes);
        Task<Attendance> CheckOutAsync(int attendanceId);
        Task<bool> HasCheckedInTodayAsync(int employeeId);
    }
}