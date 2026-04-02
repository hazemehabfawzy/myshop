using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechVault.API.Models.Entities
{
    public class ProductSpecification
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Required]
        public string SpecificationsJson { get; set; } = "{}";
    }
}
