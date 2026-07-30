using System.ComponentModel.DataAnnotations;

namespace NetPulse.Application.Options;

public class TelegramOptions
{
    public const string SectionName = "Telegram";

    [Required]
    public string BotToken { get; set; } = string.Empty;

    [Required]
    public string ChatId { get; set; } = string.Empty;
}