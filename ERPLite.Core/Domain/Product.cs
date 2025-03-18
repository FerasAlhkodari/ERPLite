using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Sku { get; set; } // Changed from SKU to Sku for consistency
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal UnitCost { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Barcode { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Inventory Inventory { get; set; }
        public ICollection<InventoryTransaction> Transactions { get; set; }
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }
}