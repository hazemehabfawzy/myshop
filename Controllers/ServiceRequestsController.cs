using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechVault.API.DTOs.ServiceRequest;
using TechVault.API.Models.Entities;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Controllers
{
    [ApiController]
    [Route("api/service-requests")]
    [Authorize]
    public class ServiceRequestsController : BaseTechVaultController
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestsController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<ActionResult<IEnumerable<ServiceRequestResponseDto>>> GetAll()
        {
            var result = await _serviceRequestService.GetAllServiceRequestsAsync();
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ServiceRequestResponseDto>>> GetMyRequests()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);

            var result = await _serviceRequestService.GetServiceRequestsByUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequestResponseDto>> GetById(Guid id)
        {
            var request = await _serviceRequestService.GetServiceRequestByIdAsync(id);
            if (request == null) return NotFound();

            // Check if user is owner, admin or technician
            if (!IsAdmin && !IsTechnician && request.UserId != CurrentUserId) return Forbid();

            return Ok(request);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequestResponseDto>> Create(CreateServiceRequestDto dto)
        {
            var result = await _serviceRequestService.CreateServiceRequestAsync(CurrentUserId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateServiceRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.Status)) return BadRequest(new { message = "Status is required." });
            var result = await _serviceRequestService.UpdateServiceRequestStatusAsync(id, dto.Status, dto.TechnicianNotes);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Assign(Guid id, [FromBody] UpdateServiceRequestDto dto)
        {
            if (!dto.TechnicianId.HasValue) return BadRequest(new { message = "TechnicianId is required." });
            var result = await _serviceRequestService.AssignTechnicianAsync(id, dto.TechnicianId.Value);
            if (!result) return BadRequest(new { message = "Could not assign technician. Check if technician exists and has proper role." });
            return NoContent();
        }
    }
}
