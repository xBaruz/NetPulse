using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using NetPulse.Application.Models;
using NetPulse.Application.Options;

namespace NetPulse.Worker.Workers;

public class LogWatcherWorker : BackgroundService
{
    private readonly ChannelWriter<RawLogLine> _channelWriter;
    private readonly WatcherOptions _options;
    private readonly ILogger<LogWatcherWorker> _logger;

    private readonly ConcurrentDictionary<string, long> _filePositions = new();

    public LogWatcherWorker(
        ChannelWriter<RawLogLine> channelWriter,
        IOptions<WatcherOptions> options,
        ILogger<LogWatcherWorker> logger)
    {
        _channelWriter = channelWriter;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!Directory.Exists(_options.DirectoryPath))
        {
            Directory.CreateDirectory(_options.DirectoryPath);
        }

        using var watcher = new FileSystemWatcher(_options.DirectoryPath, _options.Filter)
        {
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
        };

        watcher.Changed += (sender, e) => _ = OnFileChangedAsync(e.FullPath, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnFileChangedAsync(string filePath, CancellationToken ct)
    {
        try
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            long lastPosition = _filePositions.GetOrAdd(filePath, fs.Length);

            if (fs.Length < lastPosition)
            {
                lastPosition = 0;
            }

            fs.Position = lastPosition;

            using var sr = new StreamReader(fs);
            string? line;
            while ((line = await sr.ReadLineAsync(ct)) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    await _channelWriter.WriteAsync(new RawLogLine(filePath, line), ct);
                }
            }

            _filePositions[filePath] = fs.Position;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd odczytu pliku {FilePath}", filePath);
        }
    }
}