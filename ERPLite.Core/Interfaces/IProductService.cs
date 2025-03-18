using ERPLite.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPLite.Core.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync(string searchTerm = null, string category = null, bool? isActive = null);
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product, int initialQuantity, int minimumStockLevel, string locationId);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}