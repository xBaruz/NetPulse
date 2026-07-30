using System.Text.RegularExpressions;
using NetPulse.Domain.Entities;
using NetPulse.Domain.Interfaces;

namespace NetPulse.Application.Services;

public partial class LogAnalyzerService
{
    private readonly INotificationService _notificationService;
    private readonly ILogRepository _logRepository;

    public LogAnalyzerService(
        INotificationService notificationService,
        ILogRepository logRepository
    )
    {
        _notificationService = notificationService;
        _logRepository = logRepository;
    }

    [GeneratedRegex(@"^(?:\[.*?\]\s)?(ERROR|CRITICAL|FATAL):\s+(.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex LogRegex();

    public async Task ProcessAsync(string filePath, string line, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(line))
            return;
        
        var match = LogRegex().Match(line);

        if(match.Success)
        {
            var level = match.Groups[1].Value.ToUpper();
            var message = match.Groups[2].Value;

            var logEntry = LogEntry.NewLogEntry(filePath, level, message);

            await _logRepository.AddAsync(logEntry, ct);
            await _notificationService.SendAlertAsync(logEntry, ct);
        }
    }
}