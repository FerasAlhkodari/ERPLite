using ERPLite.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPLite.Core.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync(string status = null);
        Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder order);
        Task<PurchaseOrder> AddItemToPurchaseOrderAsync(int orderId, PurchaseOrderItem item);
        Task<PurchaseOrder> SubmitPurchaseOrderAsync(int id);
        Task<PurchaseOrder> ApprovePurchaseOrderAsync(int id, int approverId);
        Task<PurchaseOrder> ReceivePurchaseOrderAsync(int id, int userId);
    }
}