using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface IClassService
{
    Task<IEnumerable<GymClass>> GetActiveClassesAsync();
    Task<IEnumerable<GymClass>> GetAllAsync();
    Task<GymClass?> GetByIdAsync(int id);
    Task CreateAsync(GymClass gymClass);
    Task UpdateAsync(GymClass gymClass);
    Task DeleteAsync(int id);
}
