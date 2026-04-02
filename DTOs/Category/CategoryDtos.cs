using System.ComponentModel.DataAnnotations;

namespace TechVault.API.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public Guid? ParentCategoryId { get; set; }
    }

    public class UpdateCategoryDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public Guid? ParentCategoryId { get; set; }
    }

    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public List<CategoryResponseDto> SubCategories { get; set; } = new List<CategoryResponseDto>();
        public int ProductCount { get; set; }
    }
}
