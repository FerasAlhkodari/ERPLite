using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class PurchaseOrder : BaseEntity
    {
        public string PoNumber { get; set; } // Changed from PONumber to PoNumber for consistency
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public string Status { get; set; }  // Draft, Pending, Approved, Received
        public decimal TotalAmount { get; set; }
        public int CreatedById { get; set; }
        public int? ApprovedById { get; set; }

        // Navigation properties
        public Supplier Supplier { get; set; }
        public User CreatedBy { get; set; }
        public User ApprovedBy { get; set; }
        public ICollection<PurchaseOrderItem> Items { get; set; }
    }
}