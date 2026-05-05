using System.ComponentModel.DataAnnotations;
using TechVault.API.Models.Entities;

namespace TechVault.API.DTOs.ServiceRequest
{
    public class CreateServiceRequestDto
    {
        [Required, MaxLength(100)]
        public string DeviceType { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string IssueDescription { get; set; } = string.Empty;

        [Required]
        public string ServiceType { get; set; } = string.Empty;
    }

    public class UpdateServiceRequestDto
    {
        public string? Status { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? FinalCost { get; set; }
        public string? TechnicianNotes { get; set; }
        public Guid? TechnicianId { get; set; }
    }

    public class ServiceRequestResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string IssueDescription { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? EstimatedCost { get; set; }
        public decimal? FinalCost { get; set; }
        public string? TechnicianNotes { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime CreatedAt => ReceivedAt;
        public DateTime? CompletedAt { get; set; }
        public Guid? TechnicianId { get; set; }
        public string? TechnicianName { get; set; }
    }
}
