using System.ComponentModel.DataAnnotations;

namespace NetPulse.Application.Options;

public class WatcherOptions
{
    public const string SectionName = "Watcher";

    [Required]
    public string DirectoryPath { get; set; } = string.Empty;
    public string Filter { get; set; } = "*.log";
}