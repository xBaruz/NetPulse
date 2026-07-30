using Microsoft.EntityFrameworkCore;
using NetPulse.Domain.Entities;

namespace NetPulse.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<LogEntry> Logs => Set<LogEntry>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<LogEntry>().HasKey(x => x.Id);
        modelBuilder.Entity<LogEntry>().Property(x => x.Level).HasMaxLength(20);
        modelBuilder.Entity<LogEntry>().Property(x => x.FilePath).IsRequired();
        modelBuilder.Entity<LogEntry>().Property(x => x.Message).IsRequired();
    }
}