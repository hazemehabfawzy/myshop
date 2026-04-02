using System.ComponentModel.DataAnnotations.Schema;

namespace TechVault.API.Models.Entities
{
    public class ProductTag
    {
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("Tag")]
        public Guid TagId { get; set; }
        public virtual Tag Tag { get; set; } = null!;
    }
}
