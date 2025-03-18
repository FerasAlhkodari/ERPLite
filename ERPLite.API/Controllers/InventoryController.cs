using ERPLite.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPLite.Infrastructure.Data;

namespace ERPLite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ToListAsync();
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetLowStock()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Quantity <= i.MinimumStockLevel)
                .ToListAsync();
        }

        [HttpPost("adjust")]
        [Authorize(Roles = "Admin,InventoryManager")]
        public async Task<IActionResult> AdjustInventory(int productId, int quantity, string reason)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (inventory == null) return NotFound();

            // Update inventory
            inventory.Quantity += quantity;
            inventory.LastUpdated = DateTime.UtcNow;

            // Create transaction record
            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                Quantity = quantity,
                TransactionType = quantity > 0 ? "Addition" : "Reduction",
                TransactionDate = DateTime.UtcNow,
                Notes = reason,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            };

            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetTransactions()
        {
            return await _context.InventoryTransactions
                .Include(t => t.Product)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}