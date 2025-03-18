using ERPLite.Core.Domain;
using ERPLite.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPLite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            return await _context.Expenses
                .Include(e => e.Department)
                .Include(e => e.Requester)
                .Include(e => e.Budget)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _context.Expenses
                .Include(e => e.Department)
                .Include(e => e.Requester)
                .Include(e => e.Budget)
                .Include(e => e.Approvals)
                    .ThenInclude(a => a.Approver)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null) return NotFound();
            return expense;
        }

        [HttpGet("my-expenses")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetMyExpenses()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var employeeId = await _context.Employees
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            return await _context.Expenses
                .Where(e => e.RequesterId == employeeId)
                .Include(e => e.Department)
                .Include(e => e.Budget)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        [HttpGet("pending-approval")]
        [Authorize(Roles = "Admin,DepartmentManager,FinanceManager")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetPendingApprovals()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null) return BadRequest("Employee record not found for user");

            // For department managers, show expenses from their department
            if (User.IsInRole("DepartmentManager"))
            {
                var managedDepartmentId = await _context.Departments
                    .Where(d => d.ManagerId == employee.Id)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();

                if (managedDepartmentId == 0) return BadRequest("No managed department found");

                return await _context.Expenses
                    .Where(e => e.DepartmentId == managedDepartmentId && e.Status == "Pending")
                    .Include(e => e.Department)
                    .Include(e => e.Requester)
                    .Include(e => e.Budget)
                    .OrderByDescending(e => e.ExpenseDate)
                    .ToListAsync();
            }

            // For finance managers and admins, show all pending expenses
            return await _context.Expenses
                .Where(e => e.Status == "Pending")
                .Include(e => e.Department)
                .Include(e => e.Requester)
                .Include(e => e.Budget)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpense(Expense expense)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null) return BadRequest("Employee record not found for user");

            expense.RequesterId = employee.Id;
            expense.ExpenseDate = DateTime.UtcNow;
            expense.Status = "Pending";

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin,DepartmentManager,FinanceManager")]
        public async Task<IActionResult> ApproveExpense(int id, string comments)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var approver = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (approver == null) return BadRequest("Approver record not found");

            // Create approval record
            var approval = new ExpenseApproval
            {
                ExpenseId = id,
                ApproverId = approver.Id,
                ApprovalDate = DateTime.UtcNow,
                Status = "Approved",
                Comments = comments
            };

            _context.ExpenseApprovals.Add(approval);

            // Update expense status
            expense.Status = "Approved";

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin,DepartmentManager,FinanceManager")]
        public async Task<IActionResult> RejectExpense(int id, string comments)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var approver = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (approver == null) return BadRequest("Approver record not found");

            // Create approval record
            var approval = new ExpenseApproval
            {
                ExpenseId = id,
                ApproverId = approver.Id,
                ApprovalDate = DateTime.UtcNow,
                Status = "Rejected",
                Comments = comments
            };

            _context.ExpenseApprovals.Add(approval);

            // Update expense status
            expense.Status = "Rejected";

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}