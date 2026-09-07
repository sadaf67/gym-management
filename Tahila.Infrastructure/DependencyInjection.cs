using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tahila.Application.Services.Implementations;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Interfaces;
using Tahila.Infrastructure.Data;
using Tahila.Infrastructure.FileUpload;
using Tahila.Infrastructure.Payments;
using Tahila.Infrastructure.Repositories;

namespace Tahila.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<TahilaDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<ISiteSettingService, SiteSettingService>();
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<ICoachService, CoachService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IPaymentService, ZarinPalService>();
        services.AddScoped<IBodyAnalysisService, BodyAnalysisService>();
        services.AddScoped<IDietPlanService, DietPlanService>();
        services.AddScoped<IOfflineClassService, OfflineClassService>();
        services.AddScoped<INutritionService, NutritionService>();
        services.AddScoped<IWorkoutService, WorkoutService>();
        services.AddScoped<ITransformationService, TransformationService>();

        services.AddHttpClient<ZarinPalService>();

        services.AddScoped<FileUploadService>(sp =>
        {
            var env = sp.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            return new FileUploadService(env.WebRootPath);
        });

        return services;
    }
}
