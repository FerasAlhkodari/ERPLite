using ERPLite.Core.Domain;
using ERPLite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPLite.Infrastructure.ServiceComponents
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Supplier>> GetAllSuppliersAsync(string searchTerm = null, bool? isActive = null)
        {
            var query = _unitOfWork.SupplierRepository.GetAll();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s =>
                    s.Name.Contains(searchTerm) ||
                    s.ContactPerson.Contains(searchTerm) ||
                    s.Email.Contains(searchTerm));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            return await _unitOfWork.SupplierRepository.GetByIdAsync(id);
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            supplier.IsActive = true;
            await _unitOfWork.SupplierRepository.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();
            return supplier;
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _unitOfWork.SupplierRepository.GetByIdAsync(supplier.Id);

            if (existingSupplier == null)
                throw new Exception($"Supplier with ID {supplier.Id} not found");

            existingSupplier.Name = supplier.Name;
            existingSupplier.ContactPerson = supplier.ContactPerson;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Address = supplier.Address;
            existingSupplier.IsActive = supplier.IsActive;

            _unitOfWork.SupplierRepository.Update(existingSupplier);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id);

            if (supplier == null)
                throw new Exception($"Supplier with ID {id} not found");

            // Instead of hard delete, set as inactive
            supplier.IsActive = false;

            _unitOfWork.SupplierRepository.Update(supplier);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}