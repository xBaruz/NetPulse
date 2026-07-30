using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPulse.Domain.Interfaces;
using NetPulse.Infrastructure.Data;
using NetPulse.Infrastructure.Repositories;
using NetPulse.Infrastructure.Services;

namespace NetPulse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILogRepository, SqliteLogRepository>();

        services.AddHttpClient<INotificationService, TelegramNotificationService>()
            .AddStandardResilienceHandler();

        return services;
    }
}