using TechVault.API.DTOs.Customer;
using TechVault.API.DTOs.Order;
using TechVault.API.DTOs.ServiceRequest;

namespace TechVault.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync();
        Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id);
        Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        Task<IEnumerable<OrderResponseDto>> GetCustomerOrdersAsync(Guid userId);
        Task<IEnumerable<ServiceRequestResponseDto>> GetCustomerServiceRequestsAsync(Guid userId);
    }
}
