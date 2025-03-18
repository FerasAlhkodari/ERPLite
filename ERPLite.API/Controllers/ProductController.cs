using ERPLite.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPLite.Infrastructure.Data;

namespace ERPLite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(p => p.Inventory).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();
            return product;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,InventoryManager")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            // Set default values
            product.IsActive = true;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Create inventory record
            var inventory = new Inventory
            {
                ProductId = product.Id,
                Quantity = 0,
                MinimumStockLevel = 10,
                LastUpdated = DateTime.UtcNow
            };
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,InventoryManager")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id) return BadRequest();

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Soft delete
            product.IsActive = false;
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}