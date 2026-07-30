using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPulse.Application.Models;
using NetPulse.Application.Options;
using NetPulse.Application.Services;

namespace NetPulse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TelegramOptions>()
            .BindConfiguration(TelegramOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<WatcherOptions>()
            .BindConfiguration(WatcherOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var channel = Channel.CreateUnbounded<RawLogLine>();
        services.AddSingleton(channel.Writer);
        services.AddSingleton(channel.Reader);

        services.AddScoped<LogAnalyzerService>();

        return services;
    }
}