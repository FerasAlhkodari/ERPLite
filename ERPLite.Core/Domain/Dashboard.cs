using System;
using System.Collections.Generic;

namespace ERPLite.Core.Domain
{
    public class Dashboard : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public bool IsDefault { get; set; }

        // Navigation properties
        public ICollection<DashboardWidget> Widgets { get; set; }
    }
}