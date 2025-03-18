using ERPLite.Core.Domain;
using ERPLite.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPLite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetAll([FromQuery] string status)
        {
            var orders = await _purchaseOrderService.GetAllPurchaseOrdersAsync(status);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetById(int id)
        {
            var order = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ProcurementOfficer")]
        public async Task<ActionResult<PurchaseOrder>> Create(PurchaseOrder order)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                order.CreatedById = userId;

                var result = await _purchaseOrderService.CreatePurchaseOrderAsync(order);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/items")]
        [Authorize(Roles = "Admin,ProcurementOfficer")]
        public async Task<ActionResult<PurchaseOrder>> AddItem(int id, PurchaseOrderItem item)
        {
            try
            {
                var order = await _purchaseOrderService.AddItemToPurchaseOrderAsync(id, item);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/submit")]
        [Authorize(Roles = "Admin,ProcurementOfficer")]
        public async Task<ActionResult<PurchaseOrder>> SubmitForApproval(int id)
        {
            try
            {
                var order = await _purchaseOrderService.SubmitPurchaseOrderAsync(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin,FinanceManager")]
        public async Task<ActionResult<PurchaseOrder>> Approve(int id)
        {
            try
            {
                int approverId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var order = await _purchaseOrderService.ApprovePurchaseOrderAsync(id, approverId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/receive")]
        [Authorize(Roles = "Admin,InventoryManager")]
        public async Task<ActionResult<PurchaseOrder>> ReceiveOrder(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var order = await _purchaseOrderService.ReceivePurchaseOrderAsync(id, userId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}