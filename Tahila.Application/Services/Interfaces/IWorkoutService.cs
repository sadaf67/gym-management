using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface IWorkoutService
{
    Task<IEnumerable<WorkoutArticle>> GetAllAsync(string? category = null);
    Task<WorkoutArticle?> GetBySlugAsync(string slug);
    Task IncrementViewAsync(int id);
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<WorkoutArticle>> GetFeaturedAsync(int count = 3);
    Task<IEnumerable<WorkoutArticle>> GetRelatedAsync(int id, string category, int count = 3);

    // Admin
    Task<IEnumerable<WorkoutArticle>> GetAllAdminAsync();
    Task<WorkoutArticle?> GetByIdAsync(int id);
    Task CreateAsync(WorkoutArticle article);
    Task UpdateAsync(WorkoutArticle article);
    Task DeleteAsync(int id);
}
