using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechVault.API.Data;
using TechVault.API.DTOs.Category;
using TechVault.API.Models.Entities;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Include(c => c.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetSubcategoriesAsync(Guid parentId)
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == parentId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            category.Id = Guid.NewGuid();

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto?> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            _mapper.Map(dto, category);
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            // Optional: Handle linked products / subcategories
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
