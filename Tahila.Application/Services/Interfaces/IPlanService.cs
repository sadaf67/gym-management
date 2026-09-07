using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface IPlanService
{
    Task<IEnumerable<Plan>> GetActivePlansAsync();
    Task<IEnumerable<Plan>> GetAllAsync();
    Task<Plan?> GetByIdAsync(int id);
    Task CreateAsync(Plan plan);
    Task UpdateAsync(Plan plan);
    Task DeleteAsync(int id);
}
