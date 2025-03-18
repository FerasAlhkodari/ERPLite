using ERPLite.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPLite.Infrastructure.Data;

namespace ERPLite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Budget>>> GetBudgets()
        {
            return await _context.Budgets
                .Include(b => b.Department)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(int id)
        {
            var budget = await _context.Budgets
                .Include(b => b.Department)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (budget == null) return NotFound();
            return budget;
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<Budget>>> GetDepartmentBudgets(int departmentId)
        {
            return await _context.Budgets
                .Where(b => b.DepartmentId == departmentId)
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,FinanceManager")]
        public async Task<ActionResult<Budget>> CreateBudget(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,FinanceManager")]
        public async Task<IActionResult> UpdateBudget(int id, Budget budget)
        {
            if (id != budget.Id) return BadRequest();

            _context.Entry(budget).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.Id == id);
        }
    }
}