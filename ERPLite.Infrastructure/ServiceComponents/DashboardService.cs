using ERPLite.Core.Domain;
using ERPLite.Core.Interfaces;
using ERPLite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPLite.Infrastructure.ServiceComponents
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public DashboardService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<List<Dashboard>> GetUserDashboardsAsync(int userId)
        {
            return await _unitOfWork.DashboardRepository.GetAll()
                .Where(d => d.UserId == userId)
                .Include(d => d.Widgets)
                .ToListAsync();
        }

        public async Task<Dashboard> GetDashboardByIdAsync(int id)
        {
            return await _unitOfWork.DashboardRepository.GetAll()
                .Include(d => d.Widgets)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Dashboard> CreateDashboardAsync(Dashboard dashboard)
        {
            await _unitOfWork.DashboardRepository.AddAsync(dashboard);
            await _unitOfWork.SaveChangesAsync();
            return dashboard;
        }

        public async Task UpdateDashboardAsync(Dashboard dashboard)
        {
            _unitOfWork.DashboardRepository.Update(dashboard);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDashboardAsync(int id)
        {
            var dashboard = await _unitOfWork.DashboardRepository.GetByIdAsync(id);
            if (dashboard != null)
            {
                _unitOfWork.DashboardRepository.Delete(dashboard);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<DashboardWidget> AddWidgetAsync(int dashboardId, DashboardWidget widget)
        {
            widget.DashboardId = dashboardId;
            await _unitOfWork.DashboardWidgetRepository.AddAsync(widget);
            await _unitOfWork.SaveChangesAsync();
            return widget;
        }

        public async Task UpdateWidgetAsync(int dashboardId, DashboardWidget widget)
        {
            if (widget.DashboardId != dashboardId)
                throw new Exception("Widget does not belong to the specified dashboard");

            _unitOfWork.DashboardWidgetRepository.Update(widget);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteWidgetAsync(int widgetId)
        {
            var widget = await _unitOfWork.DashboardWidgetRepository.GetByIdAsync(widgetId);
            if (widget != null)
            {
                _unitOfWork.DashboardWidgetRepository.Delete(widget);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, object>> GetWidgetDataAsync(int widgetId)
        {
            var widget = await _unitOfWork.DashboardWidgetRepository.GetByIdAsync(widgetId);

            if (widget == null)
                throw new Exception($"Widget with ID {widgetId} not found");

            // This is where you would fetch actual data based on the widget's data source
            // For demo purposes, we'll generate sample data based on widget type

            var result = new Dictionary<string, object>();

            switch (widget.WidgetType?.ToLower() ?? "")
            {
                case "kpi":
                    result.Add("value", 85);
                    result.Add("previousValue", 75);
                    result.Add("target", 90);
                    result.Add("trend", "up");
                    break;

                case "chart":
                    var labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
                    var data = new[] { 63, 51, 88, 90, 42, 31 };
                    result.Add("labels", labels);
                    result.Add("data", data);
                    result.Add("chartType", widget.ChartType);
                    break;

                case "table":
                    var rows = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object> { { "name", "Feras Majed" }, { "department", "IT" }, { "status", "Active" } },
                        new Dictionary<string, object> { { "name", "Omar Mohammed" }, { "department", "HR" }, { "status", "Active" } },
                        new Dictionary<string, object> { { "name", "Khaled Ali" }, { "department", "Finance" }, { "status", "Inactive" } }
                    };
                    result.Add("columns", new[] { "name", "department", "status" });
                    result.Add("rows", rows);
                    break;
            }

            return result;
        }
    }
}