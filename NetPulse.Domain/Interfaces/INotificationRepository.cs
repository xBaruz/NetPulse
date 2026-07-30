using NetPulse.Domain.Entities;

namespace NetPulse.Domain.Interfaces;

public interface INotificationService
{
    Task SendAlertAsync(LogEntry log, CancellationToken cancellationToken = default);
}