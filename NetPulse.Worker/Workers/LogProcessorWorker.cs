using System.Threading.Channels;
using NetPulse.Application.Models;
using NetPulse.Application.Services;

namespace NetPulse.Worker.Workers;

public class LogProcessorWorker : BackgroundService
{
    private readonly ChannelReader<RawLogLine> _channelReader;
    private readonly IServiceProvider _serviceProvider;

    public LogProcessorWorker(ChannelReader<RawLogLine> channelReader, IServiceProvider serviceProvider)
    {
        _channelReader = channelReader;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var logLine in _channelReader.ReadAllAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var analyzer = scope.ServiceProvider.GetRequiredService<LogAnalyzerService>();

            try
            {
                await analyzer.ProcessAsync(logLine.FilePath, logLine.Content, stoppingToken);
            }
            catch
            {
            }
        }
    }
}