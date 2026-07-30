using NetPulse.Domain.Entities;

namespace NetPulse.Domain.Interfaces;

public interface ILogRepository
{
    Task AddAsync(LogEntry log, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LogEntry>> GetRecentLogsAsync(int count, CancellationToken cancellationToken = default);
}