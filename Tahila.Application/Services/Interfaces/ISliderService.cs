using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface ISliderService
{
    Task<IEnumerable<Slider>> GetActiveSliders();
    Task<IEnumerable<Slider>> GetAllSliders();
    Task<Slider?> GetByIdAsync(int id);
    Task CreateAsync(Slider slider);
    Task UpdateAsync(Slider slider);
    Task DeleteAsync(int id);
}
