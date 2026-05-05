using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TechVault.API.Data;
using TechVault.API.DTOs.Product;
using TechVault.API.Helpers;
using TechVault.API.Models.Entities;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ProductResponseDto>> GetAllProductsAsync(Guid? categoryId, decimal? minPrice, decimal? maxPrice, string? brand, string? search, bool? inStock, int pageNumber, int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Specification)
                .AsNoTracking()
                .Where(p => p.IsActive);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(p => p.Brand.Contains(brand));

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search) || (p.Model != null && p.Model.Contains(search)));
            }

            if (inStock.HasValue && inStock.Value)
                query = query.Where(p => p.StockQuantity > 0);

            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var responseItems = _mapper.Map<List<ProductResponseDto>>(items);

            return new PaginatedList<ProductResponseDto>(responseItems, count, pageNumber, pageSize);
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Specification)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(Guid categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Specification)
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(dto.SpecificationsJson))
            {
                product.Specification = new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    SpecificationsJson = dto.SpecificationsJson
                };
            }

            if (dto.TagIds != null && dto.TagIds.Any())
            {
                foreach (var tagId in dto.TagIds)
                {
                    product.ProductTags.Add(new ProductTag { ProductId = product.Id, TagId = tagId });
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _context.Products
                .Include(p => p.Specification)
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            _mapper.Map(dto, product);

            if (dto.SpecificationsJson != null)
            {
                if (product.Specification != null)
                {
                    product.Specification.SpecificationsJson = dto.SpecificationsJson;
                }
                else
                {
                    product.Specification = new ProductSpecification
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        SpecificationsJson = dto.SpecificationsJson
                    };
                }
            }

            if (dto.TagIds != null)
            {
                product.ProductTags.Clear();
                foreach (var tagId in dto.TagIds)
                {
                    product.ProductTags.Add(new ProductTag { ProductId = product.Id, TagId = tagId });
                }
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedList<ProductResponseDto>> SearchProductsAsync(string query, int pageNumber, int pageSize)
        {
            var q = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .AsNoTracking()
                .Where(p => p.IsActive && (p.Name.Contains(query) || p.Brand.Contains(query) || p.Model.Contains(query)));

            var count = await q.CountAsync();
            var items = await q.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var responseItems = _mapper.Map<List<ProductResponseDto>>(items);

            return new PaginatedList<ProductResponseDto>(responseItems, count, pageNumber, pageSize);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetLowStockProductsAsync(int threshold)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.IsActive && p.StockQuantity <= threshold)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }
    }
}
