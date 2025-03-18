using System;
using System.Collections.Generic;

namespace ERPLite.Core.Domain
{
    public class Report : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // e.g., "HR", "Inventory", "Finance"
        public string QueryDefinition { get; set; } // Stored query or procedure name
        public bool IsPublic { get; set; }
        public int CreatedById { get; set; }
        public string Format { get; set; } // "PDF", "Excel", "CSV"
    }
}