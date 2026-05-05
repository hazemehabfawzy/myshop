using TechVault.API.DTOs.Product;
using TechVault.API.Helpers;

namespace TechVault.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedList<ProductResponseDto>> GetAllProductsAsync(Guid? categoryId, decimal? minPrice, decimal? maxPrice, string? brand, string? search, bool? inStock, int pageNumber, int pageSize);
        Task<ProductResponseDto?> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(Guid categoryId);
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductResponseDto?> UpdateProductAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(Guid id); // Soft delete
        Task<PaginatedList<ProductResponseDto>> SearchProductsAsync(string query, int pageNumber, int pageSize);
        Task<IEnumerable<ProductResponseDto>> GetLowStockProductsAsync(int threshold);
    }
}
