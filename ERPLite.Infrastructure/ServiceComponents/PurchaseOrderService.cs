using ERPLite.Core.Domain;
using ERPLite.Core.Interfaces;
using ERPLite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPLite.Infrastructure.ServiceComponents
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public PurchaseOrderService(IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync(string status = null)
        {
            var query = _unitOfWork.PurchaseOrderRepository.GetAll();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(po => po.Status == status);
            }

            var orders = await query.OrderByDescending(po => po.OrderDate).ToListAsync();

            // Manually load related data
            foreach (var order in orders)
            {
                // Load supplier
                var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(order.SupplierId);
                order.Supplier = supplier;

                // Load items
                var items = await _unitOfWork.PurchaseOrderItemRepository.GetAll()
                    .Where(i => i.PurchaseOrderId == order.Id)
                    .ToListAsync();

                order.Items = items;

                // Load products for each item
                foreach (var item in items)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                    item.Product = product;
                }
            }

            return orders;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.PurchaseOrderRepository.GetByIdAsync(id);

            if (order != null)
            {
                // Load supplier
                var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(order.SupplierId);
                order.Supplier = supplier;

                // Load items
                var items = await _unitOfWork.PurchaseOrderItemRepository.GetAll()
                    .Where(i => i.PurchaseOrderId == order.Id)
                    .ToListAsync();

                order.Items = items;

                // Load products for each item
                foreach (var item in items)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                    item.Product = product;
                }
            }

            return order;
        }

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder order)
        {
            // Generate PO number
            order.PoNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Draft";
            order.TotalAmount = 0; // Will be updated as items are added

            await _unitOfWork.PurchaseOrderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return order;
        }

        public async Task<PurchaseOrder> AddItemToPurchaseOrderAsync(int orderId, PurchaseOrderItem item)
        {
            var order = await GetPurchaseOrderByIdAsync(orderId);

            if (order == null)
                throw new Exception($"Purchase Order with ID {orderId} not found");

            if (order.Status != "Draft")
                throw new Exception("Cannot add items to a purchase order that is not in Draft status");

            // Calculate line total
            item.PurchaseOrderId = orderId;
            item.LineTotal = item.Quantity * item.UnitPrice;

            await _unitOfWork.PurchaseOrderItemRepository.AddAsync(item);

            // Update order total amount
            order.TotalAmount += item.LineTotal;
            _unitOfWork.PurchaseOrderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            return await GetPurchaseOrderByIdAsync(orderId);
        }

        public async Task<PurchaseOrder> SubmitPurchaseOrderAsync(int id)
        {
            var order = await GetPurchaseOrderByIdAsync(id);

            if (order == null)
                throw new Exception($"Purchase Order with ID {id} not found");

            if (order.Status != "Draft")
                throw new Exception("Only purchase orders in Draft status can be submitted");

            if (order.Items == null || !order.Items.Any())
                throw new Exception("Cannot submit an empty purchase order");

            order.Status = "Pending";
            _unitOfWork.PurchaseOrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return order;
        }

        public async Task<PurchaseOrder> ApprovePurchaseOrderAsync(int id, int approverId)
        {
            var order = await GetPurchaseOrderByIdAsync(id);

            if (order == null)
                throw new Exception($"Purchase Order with ID {id} not found");

            if (order.Status != "Pending")
                throw new Exception("Only purchase orders in Pending status can be approved");

            order.Status = "Approved";
            order.ApprovedById = approverId;

            _unitOfWork.PurchaseOrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return order;
        }

        public async Task<PurchaseOrder> ReceivePurchaseOrderAsync(int id, int userId)
        {
            var order = await GetPurchaseOrderByIdAsync(id);

            if (order == null)
                throw new Exception($"Purchase Order with ID {id} not found");

            if (order.Status != "Approved")
                throw new Exception("Only approved purchase orders can be received");

            // Update inventory for each item
            foreach (var item in order.Items)
            {
                var inventory = await _unitOfWork.InventoryRepository.GetAll()
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

                if (inventory == null)
                {
                    // Create inventory if it doesn't exist
                    inventory = new Inventory
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        MinimumStockLevel = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                    await _unitOfWork.InventoryRepository.AddAsync(inventory);
                }
                else
                {
                    // Update existing inventory
                    inventory.Quantity += item.Quantity;
                    inventory.LastUpdated = DateTime.UtcNow;
                    _unitOfWork.InventoryRepository.Update(inventory);
                }

                // Create inventory transaction
                var transaction = new InventoryTransaction
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TransactionType = "Inbound",
                    TransactionDate = DateTime.UtcNow,
                    ReferenceId = order.Id.ToString(),
                    ReferenceType = "PurchaseOrder",
                    UserId = userId,
                    Notes = $"Received from PO #{order.PoNumber}" // Fixed property name case
                };

                await _unitOfWork.InventoryTransactionRepository.AddAsync(transaction);
            }

            // Update order status
            order.Status = "Received";
            _unitOfWork.PurchaseOrderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            return order;
        }
    }
}