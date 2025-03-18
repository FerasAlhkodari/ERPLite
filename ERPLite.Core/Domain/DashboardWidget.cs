using System;

namespace ERPLite.Core.Domain
{
    public class DashboardWidget : BaseEntity
    {
        public int DashboardId { get; set; }
        public Dashboard Dashboard { get; set; }
        public string Title { get; set; }
        public string WidgetType { get; set; } // "Chart", "KPI", "Table"
        public string ChartType { get; set; } // "Line", "Bar", "Pie"
        public string DataSource { get; set; } // Query or procedure name
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Configuration { get; set; } // JSON configuration
    }
}