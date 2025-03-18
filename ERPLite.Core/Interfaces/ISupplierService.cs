using ERPLite.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPLite.Core.Interfaces
{
    public interface ISupplierService
    {
        Task<List<Supplier>> GetAllSuppliersAsync(string searchTerm = null, bool? isActive = null);
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int id);
    }
}