using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPLite.Core.Domain
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}