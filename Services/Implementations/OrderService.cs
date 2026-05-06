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
            if (dto.PaymentMethod != "CashOnDelivery")
            {
                throw new ArgumentException("Only 'CashOnDelivery' is supported as a payment method.");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product {item.ProductId} not found");
                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Insufficient stock for {product.Name}");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Product = product,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };
                total += orderItem.UnitPrice * orderItem.Quantity;
                orderItems.Add(orderItem);

                // Reduce stock
                product.StockQuantity -= item.Quantity;
                if (product.StockQuantity <= 0)
                {
                    product.StockQuantity = 0;
                    product.StockStatus = StockStatus.OutOfStock;
                }
                else if (product.StockQuantity < 10)
                {
                    product.StockStatus = StockStatus.LowStock;
                }
                else
                {
                    product.StockStatus = StockStatus.InStock;
                }
            }

            order.TotalAmount = total;
            _context.Orders.Add(order);
            
            try
            {
                // Step 1: Save the order first to get its ID in the database
                await _context.SaveChangesAsync();

                // Step 2: Now attach and save order items with the valid OrderId
                foreach (var item in orderItems)
                {
                    item.OrderId = order.Id;
                    _context.OrderItems.Add(item);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"[OrderService ERROR] SaveChanges failed: {innerMsg}");
                throw new Exception($"Database save failed: {innerMsg}", ex);
            }

            order.OrderItems = orderItems;
            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
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
            if (order.Status != "Pending" && order.Status != "Processing") return false;

            order.Status = "Cancelled";

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
