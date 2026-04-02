using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechVault.API.Data;
using TechVault.API.DTOs.Tag;
using TechVault.API.Models.Entities;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TagService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TagResponseDto>> GetAllTagsAsync()
        {
            var tags = await _context.Tags
                .Include(t => t.ProductTags)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<TagResponseDto>>(tags);
        }

        public async Task<TagResponseDto> CreateTagAsync(TagDto dto)
        {
            var tag = _mapper.Map<Tag>(dto);
            tag.Id = Guid.NewGuid();

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return _mapper.Map<TagResponseDto>(tag);
        }

        public async Task<bool> DeleteTagAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return false;

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
