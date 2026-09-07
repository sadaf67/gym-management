using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface ICoachService
{
    Task<IEnumerable<Coach>> GetActiveCoachesAsync();
    Task<IEnumerable<Coach>> GetAllAsync();
    Task<Coach?> GetByIdAsync(int id);
    Task CreateAsync(Coach coach);
    Task UpdateAsync(Coach coach);
    Task DeleteAsync(int id);
}
