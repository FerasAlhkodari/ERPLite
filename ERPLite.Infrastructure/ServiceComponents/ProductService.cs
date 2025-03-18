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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public ProductService(IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<List<Product>> GetAllProductsAsync(string searchTerm = null, string category = null, bool? isActive = null)
        {
            // Use AsNoTracking for read-only queries for better performance
            var query = _unitOfWork.ProductRepository.GetAll().AsNoTracking();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    (p.Sku != null && p.Sku.Contains(searchTerm)) || // Fixed property name case
                    (p.Description != null && p.Description.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            // First get products without includes to avoid conversion issues
            var products = await query.ToListAsync();

            // Then manually load related data
            foreach (var product in products)
            {
                var inventory = await _unitOfWork.InventoryRepository.GetAll()
                    .FirstOrDefaultAsync(i => i.ProductId == product.Id);
                product.Inventory = inventory;
            }

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

            if (product != null)
            {
                // Manually load the inventory
                var inventory = await _unitOfWork.InventoryRepository.GetAll()
                    .FirstOrDefaultAsync(i => i.ProductId == product.Id);
                product.Inventory = inventory;
            }

            return product;
        }

        public async Task<Product> CreateProductAsync(Product product, int initialQuantity, int minimumStockLevel, string locationId)
        {
            // Create product
            product.IsActive = true;
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Create inventory record for the product
            var inventory = new Inventory
            {
                ProductId = product.Id,
                Quantity = initialQuantity,
                MinimumStockLevel = minimumStockLevel,
                LocationId = locationId != null ? int.Parse(locationId) : null,
                LastUpdated = DateTime.UtcNow
            };

            await _unitOfWork.InventoryRepository.AddAsync(inventory);

            // Create initial inventory transaction if there's initial stock
            if (initialQuantity > 0)
            {
                var transaction = new InventoryTransaction
                {
                    ProductId = product.Id,
                    Quantity = initialQuantity,
                    TransactionType = "Initial",
                    TransactionDate = DateTime.UtcNow,
                    ReferenceType = "ProductCreation",
                    Notes = "Initial inventory setup"
                };

                await _unitOfWork.InventoryTransactionRepository.AddAsync(transaction);
            }

            await _unitOfWork.SaveChangesAsync();

            // Load related inventory to return
            product.Inventory = inventory;
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(product.Id);

            if (existingProduct == null)
                throw new Exception($"Product with ID {product.Id} not found");

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.UnitCost = product.UnitCost;
            existingProduct.UnitOfMeasure = product.UnitOfMeasure;
            existingProduct.Barcode = product.Barcode;
            existingProduct.IsActive = product.IsActive;

            if (product.Sku != null) // Fixed property name case
            {
                existingProduct.Sku = product.Sku;
            }

            _unitOfWork.ProductRepository.Update(existingProduct);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

            if (product == null)
                throw new Exception($"Product with ID {id} not found");

            // Instead of hard delete, set as inactive
            product.IsActive = false;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}