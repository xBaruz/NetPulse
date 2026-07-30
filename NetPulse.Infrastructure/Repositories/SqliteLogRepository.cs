using Microsoft.EntityFrameworkCore;
using NetPulse.Domain.Entities;
using NetPulse.Domain.Interfaces;
using NetPulse.Infrastructure.Data;

namespace NetPulse.Infrastructure.Repositories;

public class SqliteLogRepository : ILogRepository
{
    private readonly AppDbContext _dbContext;

    public SqliteLogRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(LogEntry log, CancellationToken cancellationToken = default)
    {
        await _dbContext.Logs.AddAsync(log, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<LogEntry>> GetRecentLogsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Logs
            .AsNoTracking()
            .OrderByDescending(x => x.DetectedAt)
            .Take(count)
            .ToListAsync();
    }
}