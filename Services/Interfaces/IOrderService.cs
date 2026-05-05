using TechVault.API.DTOs.Order;
using TechVault.API.Models.Entities;

namespace TechVault.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetOrdersByUserAsync(Guid userId);
        Task<OrderResponseDto?> GetOrderByIdAsync(Guid id);
        Task<OrderResponseDto> CreateOrderAsync(Guid userId, CreateOrderDto dto);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, string status);
        Task<bool> CancelOrderAsync(Guid orderId, Guid userId, bool isAdmin);
    }
}
