using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechVault.API.Models.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public StockStatus StockStatus { get; set; } = StockStatus.InStock;

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Model { get; set; }

        [MaxLength(50)]
        public string? SKU { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}
