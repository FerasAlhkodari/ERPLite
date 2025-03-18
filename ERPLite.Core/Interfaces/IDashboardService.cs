using ERPLite.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPLite.Core.Interfaces
{
    public interface IDashboardService
    {
        Task<List<Dashboard>> GetUserDashboardsAsync(int userId);
        Task<Dashboard> GetDashboardByIdAsync(int id);
        Task<Dashboard> CreateDashboardAsync(Dashboard dashboard);
        Task UpdateDashboardAsync(Dashboard dashboard);
        Task DeleteDashboardAsync(int id);
        Task<DashboardWidget> AddWidgetAsync(int dashboardId, DashboardWidget widget);
        Task UpdateWidgetAsync(int dashboardId, DashboardWidget widget);
        Task DeleteWidgetAsync(int widgetId);
        Task<Dictionary<string, object>> GetWidgetDataAsync(int widgetId);
    }
}