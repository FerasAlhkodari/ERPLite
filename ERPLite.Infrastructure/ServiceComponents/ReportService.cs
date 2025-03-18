using ERPLite.Core.Domain;
using ERPLite.Core.Interfaces;
using ERPLite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Infrastructure.ServiceComponents
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public ReportService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<List<Report>> GetAllReportsAsync(string category = null)
        {
            var reports = await _unitOfWork.ReportRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(category))
            {
                reports = reports.Where(r => r.Category == category).ToList();
            }

            return reports.ToList();
        }

        public async Task<Report> GetReportByIdAsync(int id)
        {
            return await _unitOfWork.ReportRepository.GetByIdAsync(id);
        }

        public async Task<Report> CreateReportAsync(Report report)
        {
            await _unitOfWork.ReportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync();
            return report;
        }

        public async Task UpdateReportAsync(Report report)
        {
            await _unitOfWork.ReportRepository.UpdateAsync(report);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteReportAsync(int id)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(id);
            if (report != null)
            {
                await _unitOfWork.ReportRepository.DeleteAsync(report);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<byte[]> GenerateEmployeeDirectoryAsync(string format = "csv")
        {
            // Instead of using Include, we'll fetch the data separately and join manually
            var employees = await _context.Employees.ToListAsync();
            var departments = await _context.Departments.ToListAsync();
            var positions = await _context.Positions.ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Employee Directory Report");
            sb.AppendLine($"Generated on: {DateTime.UtcNow}");
            sb.AppendLine("--------------------------------------------");
            sb.AppendLine("ID,Name,Department,Position,Contact");

            foreach (var employee in employees)
            {
                var department = departments.FirstOrDefault(d => d.Id == employee.DepartmentId);
                var position = positions.FirstOrDefault(p => p.Id == employee.PositionId);

                sb.AppendLine($"{employee.EmployeeId},{employee.FirstName} {employee.LastName},{department?.Name},{position?.Title},{employee.ContactNumber}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> GenerateAttendanceReportAsync(int? departmentId, DateTime startDate, DateTime endDate, string format = "csv")
        {
            // Fetch data separately and join manually
            var attendances = await _context.Attendances
                .Where(a => a.CheckIn >= startDate && a.CheckIn <= endDate)
                .ToListAsync();

            var employees = await _context.Employees.ToListAsync();
            var departments = await _context.Departments.ToListAsync();

            if (departmentId.HasValue)
            {
                employees = employees.Where(e => e.DepartmentId == departmentId.Value).ToList();
                attendances = attendances.Where(a => employees.Select(e => e.Id).Contains(a.EmployeeId)).ToList();
            }

            var orderedAttendances = attendances
                .OrderBy(a => employees.FirstOrDefault(e => e.Id == a.EmployeeId)?.LastName)
                .ThenBy(a => a.CheckIn)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Attendance Report");
            sb.AppendLine($"Period: {startDate:MM/dd/yyyy} to {endDate:MM/dd/yyyy}");
            sb.AppendLine("--------------------------------------------");
            sb.AppendLine("Date,Employee ID,Employee Name,Department,Check In,Check Out,Hours");

            foreach (var attendance in orderedAttendances)
            {
                var employee = employees.FirstOrDefault(e => e.Id == attendance.EmployeeId);
                if (employee == null) continue;

                var department = departments.FirstOrDefault(d => d.Id == employee.DepartmentId);

                var hours = 0.0;
                if (attendance.CheckOut.HasValue)
                {
                    hours = (attendance.CheckOut.Value - attendance.CheckIn).TotalHours;
                }

                sb.AppendLine($"{attendance.CheckIn:MM/dd/yyyy},{employee.EmployeeId},{employee.FirstName} {employee.LastName},{department?.Name},{attendance.CheckIn:HH:mm},{(attendance.CheckOut.HasValue ? attendance.CheckOut.Value.ToString("HH:mm") : "N/A")},{hours:F2}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> GenerateStockLevelReportAsync(string category, bool showLowStockOnly, string format = "csv")
        {
            // Fetch data separately and join manually
            var inventories = await _context.Inventories.ToListAsync();
            var products = await _context.Products.Where(p => p.IsActive).ToListAsync();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category).ToList();
                inventories = inventories.Where(i => products.Select(p => p.Id).Contains(i.ProductId)).ToList();
            }

            if (showLowStockOnly)
            {
                inventories = inventories.Where(i => i.Quantity <= i.MinimumStockLevel).ToList();
            }

            var orderedInventories = inventories
                .OrderBy(i => products.FirstOrDefault(p => p.Id == i.ProductId)?.Category)
                .ThenBy(i => products.FirstOrDefault(p => p.Id == i.ProductId)?.Name)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Stock Level Report");
            sb.AppendLine($"Generated on: {DateTime.UtcNow}");
            sb.AppendLine($"Category: {(string.IsNullOrEmpty(category) ? "All" : category)}");
            sb.AppendLine($"Low Stock Only: {(showLowStockOnly ? "Yes" : "No")}");
            sb.AppendLine("--------------------------------------------");
            sb.AppendLine("Sku,Product Name,Category,Current Stock,Min Level,Status");

            foreach (var inventory in orderedInventories)
            {
                var product = products.FirstOrDefault(p => p.Id == inventory.ProductId);
                if (product == null) continue;

                string status = inventory.Quantity <= inventory.MinimumStockLevel ? "LOW STOCK" : "OK";
                sb.AppendLine($"{product.Sku},{product.Name},{product.Category},{inventory.Quantity},{inventory.MinimumStockLevel},{status}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> GenerateExpenseReportAsync(int? departmentId, DateTime startDate, DateTime endDate, string format = "csv")
        {
            // Fetch data separately and join manually
            var expenses = await _context.Expenses
                .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                .ToListAsync();

            var departments = await _context.Departments.ToListAsync();
            var employees = await _context.Employees.ToListAsync();
            var budgets = await _context.Budgets.ToListAsync();

            if (departmentId.HasValue)
            {
                expenses = expenses.Where(e => e.DepartmentId == departmentId.Value).ToList();
            }

            var orderedExpenses = expenses
                .OrderBy(e => departments.FirstOrDefault(d => d.Id == e.DepartmentId)?.Name)
                .ThenBy(e => e.ExpenseDate)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Expense Report");
            sb.AppendLine($"Period: {startDate:MM/dd/yyyy} to {endDate:MM/dd/yyyy}");
            sb.AppendLine("--------------------------------------------");
            sb.AppendLine("Date,Department,Category,Requested By,Description,Amount,Status");

            foreach (var expense in orderedExpenses)
            {
                var department = departments.FirstOrDefault(d => d.Id == expense.DepartmentId);
                var requester = employees.FirstOrDefault(e => e.Id == expense.RequesterId);

                sb.AppendLine($"{expense.ExpenseDate:MM/dd/yyyy},{department?.Name},{expense.Category},{requester?.FirstName} {requester?.LastName},{expense.Description},{expense.Amount:C2},{expense.Status}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}