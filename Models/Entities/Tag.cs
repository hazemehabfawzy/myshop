using System.ComponentModel.DataAnnotations;

namespace TechVault.API.Models.Entities
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}
