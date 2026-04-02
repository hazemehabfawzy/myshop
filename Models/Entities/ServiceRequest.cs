using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechVault.API.Models.Entities
{
    public class ServiceRequest
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [InverseProperty("CustomerServiceRequests")]
        public virtual User User { get; set; } = null!;

        [Required, MaxLength(100)]
        public string DeviceType { get; set; } = string.Empty; // Phone | Laptop | Custom PC Build

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string IssueDescription { get; set; } = string.Empty;

        [Required]
        public ServiceType ServiceType { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Received;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? FinalCost { get; set; }

        [MaxLength(2000)]
        public string? TechnicianNotes { get; set; }

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public Guid? TechnicianId { get; set; }
        [ForeignKey("TechnicianId")]
        [InverseProperty("TechnicianServiceRequests")]
        public virtual User? Technician { get; set; }
    }
}
