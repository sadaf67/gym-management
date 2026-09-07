using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface IOfflineClassService
{
    // Public
    Task<IEnumerable<OfflineClass>> GetAllAsync();
    Task<OfflineClass?> GetByIdWithSessionsAsync(int id);
    Task<ClassSession?> GetSessionAsync(int sessionId);

    // Admin CRUD - Courses
    Task<IEnumerable<OfflineClass>> GetAllAdminAsync();
    Task<OfflineClass?> GetByIdAsync(int id);
    Task CreateAsync(OfflineClass course);
    Task UpdateAsync(OfflineClass course);
    Task DeleteAsync(int id);

    // Admin CRUD - Sessions
    Task CreateSessionAsync(ClassSession session);
    Task UpdateSessionAsync(ClassSession session);
    Task DeleteSessionAsync(int sessionId);
}
