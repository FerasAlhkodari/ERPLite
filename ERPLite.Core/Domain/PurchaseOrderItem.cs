using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class PurchaseOrderItem : BaseEntity
    {
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        // Navigation properties
        public PurchaseOrder PurchaseOrder { get; set; }
        public Product Product { get; set; }
    }
}
