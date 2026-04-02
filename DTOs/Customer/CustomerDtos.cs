using System.ComponentModel.DataAnnotations;

namespace TechVault.API.DTOs.Customer
{
    public class UpdateProfileDto
    {
        [MaxLength(200)]
        public string? FullName { get; set; }

        [Phone, MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }

    public class CustomerResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
