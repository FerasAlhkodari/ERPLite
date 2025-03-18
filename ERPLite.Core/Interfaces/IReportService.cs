using ERPLite.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPLite.Core.Interfaces
{
    public interface IReportService
    {
        Task<List<Report>> GetAllReportsAsync(string category = null);
        Task<Report> GetReportByIdAsync(int id);
        Task<Report> CreateReportAsync(Report report);
        Task UpdateReportAsync(Report report);
        Task DeleteReportAsync(int id);

        // Report generation methods
        Task<byte[]> GenerateEmployeeDirectoryAsync(string format = "csv");
        Task<byte[]> GenerateAttendanceReportAsync(int? departmentId, DateTime startDate, DateTime endDate, string format = "csv");
        Task<byte[]> GenerateStockLevelReportAsync(string category, bool showLowStockOnly, string format = "csv");
        Task<byte[]> GenerateExpenseReportAsync(int? departmentId, DateTime startDate, DateTime endDate, string format = "csv");
    }
}