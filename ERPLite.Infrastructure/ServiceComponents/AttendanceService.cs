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
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attendance>> GetAttendanceByEmployeeAsync(int employeeId)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.CheckIn)
                .ToListAsync();
        }

        public async Task<Attendance> CheckInAsync(int employeeId, string notes)
        {
            if (await HasCheckedInTodayAsync(employeeId))
                throw new InvalidOperationException("Already checked in today");

            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                CheckIn = DateTime.Now,
                Status = "Present",
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();

            return attendance;
        }

        public async Task<Attendance> CheckOutAsync(int attendanceId)
        {
            var attendance = await _context.Attendances.FindAsync(attendanceId);
            if (attendance == null)
                throw new KeyNotFoundException("Attendance record not found");

            if (attendance.CheckOut.HasValue)
                throw new InvalidOperationException("Already checked out");

            attendance.CheckOut = DateTime.Now;
            attendance.UpdatedAt = DateTime.UtcNow;

            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();

            return attendance;
        }

        public async Task<bool> HasCheckedInTodayAsync(int employeeId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _context.Attendances
                .AnyAsync(a => a.EmployeeId == employeeId &&
                              a.CheckIn >= today && a.CheckIn < tomorrow);
        }
    }
}