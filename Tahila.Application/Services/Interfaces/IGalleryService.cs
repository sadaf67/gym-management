using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface IGalleryService
{
    Task<IEnumerable<GalleryItem>> GetActiveItemsAsync();
    Task<IEnumerable<GalleryItem>> GetAllAsync();
    Task<GalleryItem?> GetByIdAsync(int id);
    Task CreateAsync(GalleryItem item);
    Task UpdateAsync(GalleryItem item);
    Task DeleteAsync(int id);
}
