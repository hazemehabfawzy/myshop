using System.ComponentModel.DataAnnotations;

namespace TechVault.API.DTOs.Tag
{
    public class TagDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }

    public class TagResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}
