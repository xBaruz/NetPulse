namespace NetPulse.Domain.Entities;

public class LogEntry
{
    public LogEntry(){}
    private LogEntry(
        Guid id,
        string filePath,
        string level,
        string message,
        DateTime detectedAt
    )
    {
        Id = id;
        FilePath = filePath;
        Level = level;
        Message = message;
        DetectedAt = detectedAt;
    }
    public Guid Id { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public string Level { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public DateTime DetectedAt { get; private set; }

    public static LogEntry NewLogEntry(
        string filePath,
        string level,
        string message
    )
    {
        return new LogEntry(
            Guid.NewGuid(),
            filePath,
            level,
            message,
            DateTime.UtcNow
        );
    }
}