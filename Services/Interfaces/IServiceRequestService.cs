using TechVault.API.DTOs.ServiceRequest;
using TechVault.API.Models.Entities;

namespace TechVault.API.Services.Interfaces
{
    public interface IServiceRequestService
    {
        Task<IEnumerable<ServiceRequestResponseDto>> GetAllServiceRequestsAsync();
        Task<IEnumerable<ServiceRequestResponseDto>> GetServiceRequestsByUserAsync(Guid userId);
        Task<ServiceRequestResponseDto?> GetServiceRequestByIdAsync(Guid id);
        Task<ServiceRequestResponseDto> CreateServiceRequestAsync(Guid userId, CreateServiceRequestDto dto);
        Task<bool> UpdateServiceRequestStatusAsync(Guid id, ServiceRequestStatus status, string? technicianNotes);
        Task<bool> AssignTechnicianAsync(Guid requestId, Guid technicianId);
    }
}
