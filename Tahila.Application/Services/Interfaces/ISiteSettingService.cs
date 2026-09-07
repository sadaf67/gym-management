using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface ISiteSettingService
{
    Task<SiteSetting> GetSettingsAsync();
    Task UpdateSettingsAsync(SiteSetting settings);
}
