using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Inventory : BaseEntity
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int MinimumStockLevel { get; set; }
        public int? LocationId { get; set; }
        public DateTime LastUpdated { get; set; }

        // Navigation property
        public Product Product { get; set; }
    }
}