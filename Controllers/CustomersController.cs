using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechVault.API.DTOs.Customer;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : BaseTechVaultController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAll()
        {
            var result = await _customerService.GetAllCustomersAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> GetById(Guid id)
        {
            // Check if user is self or admin
            if (!IsAdmin && id != CurrentUserId) return Forbid();

            var result = await _customerService.GetCustomerByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var result = await _customerService.UpdateProfileAsync(CurrentUserId, dto);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
