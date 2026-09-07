using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface IBodyAnalysisService
{
    BodyAnalysis Calculate(BodyAnalysis input);
    Task<BodyAnalysis> SaveAsync(BodyAnalysis analysis);
    Task<IEnumerable<BodyAnalysis>> GetUserHistoryAsync(string userId);
    Task<BodyAnalysis?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}
