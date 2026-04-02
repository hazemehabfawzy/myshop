using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechVault.API.Data;
using TechVault.API.DTOs.Customer;
using TechVault.API.DTOs.Order;
using TechVault.API.DTOs.ServiceRequest;
using TechVault.API.Models.Entities;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CustomerService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync()
        {
            var customers = await _context.Users
                .Include(u => u.CustomerProfile)
                .AsNoTracking()
                .Where(u => u.Role == "Customer")
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _context.Users
                .Include(u => u.CustomerProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _context.Users
                .Include(u => u.CustomerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            if (user.CustomerProfile == null)
            {
                user.CustomerProfile = new CustomerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
            }

            if (dto.FullName != null) user.CustomerProfile.FullName = dto.FullName;
            if (dto.PhoneNumber != null) user.CustomerProfile.PhoneNumber = dto.PhoneNumber;
            if (dto.Address != null) user.CustomerProfile.Address = dto.Address;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetCustomerOrdersAsync(Guid userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        }

        public async Task<IEnumerable<ServiceRequestResponseDto>> GetCustomerServiceRequestsAsync(Guid userId)
        {
            var requests = await _context.ServiceRequests
                .Include(sr => sr.User).ThenInclude(u => u.CustomerProfile)
                .Include(sr => sr.Technician).ThenInclude(u => u.CustomerProfile)
                .AsNoTracking()
                .Where(sr => sr.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ServiceRequestResponseDto>>(requests);
        }
    }
}
