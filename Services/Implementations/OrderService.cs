using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechVault.API.Data;
using TechVault.API.DTOs.Order;
using TechVault.API.Models.Entities;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserAsync(Guid userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<OrderResponseDto> CreateOrderAsync(Guid userId, CreateOrderDto dto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes,
                Status = OrderStatus.Pending
            };

            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    throw new Exception($"Product {item.ProductId} not found or insufficient stock.");
                }

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                totalAmount += orderItem.UnitPrice * orderItem.Quantity;
                product.StockQuantity -= item.Quantity; // Decrement stock

                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, Guid userId, bool isAdmin)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return false;

            // Only admin or the owner can cancel
            if (!isAdmin && order.UserId != userId) return false;

            // Can only cancel if Pending or Processing
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing) return false;

            order.Status = OrderStatus.Cancelled;

            // Refund stock
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
