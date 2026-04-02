using TechVault.API.DTOs.Category;

namespace TechVault.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id);
        Task<IEnumerable<CategoryResponseDto>> GetSubcategoriesAsync(Guid parentId);
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryResponseDto?> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
