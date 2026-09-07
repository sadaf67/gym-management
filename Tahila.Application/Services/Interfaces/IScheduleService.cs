using Tahila.Domain.Entities;
using Tahila.Domain.Enums;

namespace Tahila.Application.Services.Interfaces;

public interface IScheduleService
{
    Task<IEnumerable<Schedule>> GetAllWithClassAsync();
    Task<IEnumerable<Schedule>> GetByDayAsync(DayOfWeekPersian day);
    Task<Schedule?> GetByIdAsync(int id);
    Task CreateAsync(Schedule schedule);
    Task UpdateAsync(Schedule schedule);
    Task DeleteAsync(int id);
}
