using ERPLite.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPLite.Core.Interfaces
{
    public interface IInventoryService
    {
        Task<List<Inventory>> GetAllInventoryAsync();
        Task<List<Inventory>> GetLowStockInventoryAsync();
        Task AdjustInventoryAsync(int productId, int quantity, string reason, string transactionType, int userId);
        Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(int? productId = null, string transactionType = null);
    }
}