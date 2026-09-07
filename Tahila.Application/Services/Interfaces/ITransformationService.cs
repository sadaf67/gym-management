using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface ITransformationService
{
    Task<IEnumerable<Transformation>> GetAllActiveAsync();
    Task<IEnumerable<Transformation>> GetAllAdminAsync();
    Task<Transformation?> GetByIdAsync(int id);
    Task CreateAsync(Transformation item);
    Task UpdateAsync(Transformation item);
    Task DeleteAsync(int id);
}
