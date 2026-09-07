using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface INutritionService
{
    // Public
    Task<IEnumerable<NutritionArticle>> GetAllAsync(string? category = null);
    Task<NutritionArticle?> GetBySlugAsync(string slug);
    Task IncrementViewAsync(int id);
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<NutritionArticle>> GetFeaturedAsync(int count = 3);
    Task<IEnumerable<NutritionArticle>> GetRelatedAsync(int id, string category, int count = 3);

    // Admin CRUD
    Task<IEnumerable<NutritionArticle>> GetAllAdminAsync();
    Task<NutritionArticle?> GetByIdAsync(int id);
    Task CreateAsync(NutritionArticle article);
    Task UpdateAsync(NutritionArticle article);
    Task DeleteAsync(int id);
}
