using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechVault.API.Data;
using TechVault.API.DTOs.Order;
using TechVault.API.Models.Entities;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : BaseTechVaultController
    {
        private readonly IOrderService _orderService;
        private readonly AppDbContext _context;

        public OrdersController(IOrderService orderService, AppDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders()
        {
            var result = await _orderService.GetOrdersByUserAsync(CurrentUserId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            // Check if user is owner or admin
            if (!IsAdmin && order.UserId != CurrentUserId) return Forbid();

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create(CreateOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = CurrentUserId;
            Console.WriteLine($"[OrdersController DEBUG] Resolved userId: '{userId}'");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"[OrdersController DEBUG] Claim: Type='{claim.Type}', Value='{claim.Value}'");
            }

            if (userId == Guid.Empty)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            // Verify that user exists in dbo.Users database table before inserting
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                Console.WriteLine($"[OrdersController ERROR] UserId '{userId}' does not exist in the database!");
                return Unauthorized(new { message = "User does not exist. Please redirect to login." });
            }

            Console.WriteLine($"[OrdersController] Creating order for user {userId}. Items count: {dto.Items?.Count}, Address: {dto.ShippingAddress}, PaymentMethod: {dto.PaymentMethod}");
            
            try
            {
                var result = await _orderService.CreateOrderAsync(userId, dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"[OrdersController ERROR] Order creation failed. Message: {ex.Message}. Inner message: {innerMsg}");
                return BadRequest(new { message = ex.Message, innerMessage = innerMsg });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> Cancel(Guid id)
        {
            var result = await _orderService.CancelOrderAsync(id, CurrentUserId, IsAdmin);
            if (!result) return BadRequest(new { message = "Could not cancel order. Check order status or owner." });
            return NoContent();
        }
    }
}
