using System.ComponentModel.DataAnnotations;

namespace TechVault.API.DTOs.Product
{
    public class CreateProductDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required, Range(0.01, 1000000.00)]
        public decimal Price { get; set; }

        [Required, Range(0, 100000)]
        public int StockQuantity { get; set; }

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Model { get; set; }

        [MaxLength(50)]
        public string? SKU { get; set; }

        [Url, MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public string? SpecificationsJson { get; set; }

        public List<Guid>? TagIds { get; set; }
    }

    public class UpdateProductDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Range(0.01, 1000000.00)]
        public decimal? Price { get; set; }

        [Range(0, 100000)]
        public int? StockQuantity { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        [MaxLength(100)]
        public string? Model { get; set; }

        [MaxLength(50)]
        public string? SKU { get; set; }

        [Url, MaxLength(500)]
        public string? ImageUrl { get; set; }

        public Guid? CategoryId { get; set; }

        public bool? IsActive { get; set; }

        public string? SpecificationsJson { get; set; }

        public List<Guid>? TagIds { get; set; }
    }

    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? SKU { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Dictionary<string, string>? Specifications { get; set; }
    }
}
