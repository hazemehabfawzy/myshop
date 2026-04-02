using System.ComponentModel.DataAnnotations;

namespace TechVault.API.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = "Customer"; // Admin | Customer | Technician

        // Navigation properties
        public virtual CustomerProfile? CustomerProfile { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<ServiceRequest> CustomerServiceRequests { get; set; } = new List<ServiceRequest>();
        public virtual ICollection<ServiceRequest> TechnicianServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
