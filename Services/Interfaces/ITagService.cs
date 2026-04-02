using TechVault.API.DTOs.Tag;

namespace TechVault.API.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagResponseDto>> GetAllTagsAsync();
        Task<TagResponseDto> CreateTagAsync(TagDto dto);
        Task<bool> DeleteTagAsync(Guid id);
    }
}
