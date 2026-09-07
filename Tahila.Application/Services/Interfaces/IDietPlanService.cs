using Tahila.Application.Models;
using Tahila.Domain.Entities;

namespace Tahila.Application.Services.Interfaces;

public interface IDietPlanService
{
    DietPlanModel Generate(BodyAnalysis bmi, bool isVip);
}
