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
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public InventoryService(IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<List<Inventory>> GetAllInventoryAsync()
        {
            var inventories = await _unitOfWork.InventoryRepository.GetAllAsync();

            // Manually load products to avoid LINQ conversion issues
            foreach (var inventory in inventories)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(inventory.ProductId);
                inventory.Product = product;
            }

            return inventories.ToList();
        }

        public async Task<List<Inventory>> GetLowStockInventoryAsync()
        {
            var allInventories = await _unitOfWork.InventoryRepository.GetAllAsync();
            var lowStockInventories = allInventories
                .Where(i => i.Quantity <= i.MinimumStockLevel)
                .ToList();

            // Manually load products
            foreach (var inventory in lowStockInventories)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(inventory.ProductId);
                inventory.Product = product;
            }

            return lowStockInventories;
        }

        public async Task AdjustInventoryAsync(int productId, int quantity, string reason, string transactionType, int userId)
        {
            // Find inventory by product ID without using LINQ conversion
            var inventories = await _unitOfWork.InventoryRepository.GetAsync(i => i.ProductId == productId);
            var inventory = inventories.FirstOrDefault();

            if (inventory == null)
                throw new Exception($"Inventory for product ID {productId} not found");

            // Update inventory quantity
            inventory.Quantity += quantity; // Can be positive or negative
            inventory.LastUpdated = DateTime.UtcNow;

            await _unitOfWork.InventoryRepository.UpdateAsync(inventory);

            // Create transaction record
            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                Quantity = quantity,
                TransactionType = transactionType,
                TransactionDate = DateTime.UtcNow,
                ReferenceType = "ManualAdjustment",
                UserId = userId,
                Notes = reason
            };

            await _unitOfWork.InventoryTransactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(int? productId = null, string transactionType = null)
        {
            // Simplified query to avoid LINQ conversion issues
            var allTransactions = await _unitOfWork.InventoryTransactionRepository.GetAllAsync();

            // Apply filters in memory
            var filteredTransactions = allTransactions.AsEnumerable();

            if (productId.HasValue)
            {
                filteredTransactions = filteredTransactions.Where(t => t.ProductId == productId.Value);
            }

            if (!string.IsNullOrEmpty(transactionType))
            {
                filteredTransactions = filteredTransactions.Where(t => t.TransactionType == transactionType);
            }

            var result = filteredTransactions.OrderByDescending(t => t.TransactionDate).ToList();

            // Manually load product data
            foreach (var transaction in result)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(transaction.ProductId);
                transaction.Product = product;
            }

            return result;
        }
    }
}