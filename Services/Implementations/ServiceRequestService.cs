using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechVault.API.Data;
using TechVault.API.DTOs.ServiceRequest;
using TechVault.API.Models.Entities;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Services.Implementations
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ServiceRequestService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServiceRequestResponseDto>> GetAllServiceRequestsAsync()
        {
            var requests = await _context.ServiceRequests
                .Include(sr => sr.User).ThenInclude(u => u.CustomerProfile)
                .Include(sr => sr.Technician).ThenInclude(u => u.CustomerProfile)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ServiceRequestResponseDto>>(requests);
        }

        public async Task<IEnumerable<ServiceRequestResponseDto>> GetServiceRequestsByUserAsync(Guid userId)
        {
            var requests = await _context.ServiceRequests
                .Include(sr => sr.User).ThenInclude(u => u.CustomerProfile)
                .Include(sr => sr.Technician).ThenInclude(u => u.CustomerProfile)
                .AsNoTracking()
                .Where(sr => sr.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ServiceRequestResponseDto>>(requests);
        }

        public async Task<ServiceRequestResponseDto?> GetServiceRequestByIdAsync(Guid id)
        {
            var request = await _context.ServiceRequests
                .Include(sr => sr.User).ThenInclude(u => u.CustomerProfile)
                .Include(sr => sr.Technician).ThenInclude(u => u.CustomerProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(sr => sr.Id == id);

            return _mapper.Map<ServiceRequestResponseDto>(request);
        }

        public async Task<ServiceRequestResponseDto> CreateServiceRequestAsync(Guid userId, CreateServiceRequestDto dto)
        {
            var request = _mapper.Map<ServiceRequest>(dto);
            request.Id = Guid.NewGuid();
            request.UserId = userId;
            request.ReceivedAt = DateTime.UtcNow;
            request.Status = "Received";

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();

            return _mapper.Map<ServiceRequestResponseDto>(request);
        }

        public async Task<bool> UpdateServiceRequestStatusAsync(Guid id, string status, string? technicianNotes)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return false;

            request.Status = status;
            if (technicianNotes != null)
                request.TechnicianNotes = technicianNotes;

            if (status == "Completed")
                request.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTechnicianAsync(Guid requestId, Guid technicianId)
        {
            var request = await _context.ServiceRequests.FindAsync(requestId);
            if (request == null) return false;

            var technician = await _context.Users.FindAsync(technicianId);
            if (technician == null || (technician.Role != "Technician" && technician.Role != "Admin")) return false;

            request.TechnicianId = technicianId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
