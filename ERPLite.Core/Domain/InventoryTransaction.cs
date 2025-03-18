using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class InventoryTransaction : BaseEntity
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; }  // In, Out, Adjustment
        public DateTime TransactionDate { get; set; }
        public string ReferenceId { get; set; }  // PO number, etc.
        public string ReferenceType { get; set; }  // PurchaseOrder, SalesOrder, etc.
        public int UserId { get; set; }
        public string Notes { get; set; }

        // Navigation property
        public Product Product { get; set; }
        public User User { get; set; }
    }
}