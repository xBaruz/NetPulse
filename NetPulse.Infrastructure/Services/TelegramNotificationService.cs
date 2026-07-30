using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using NetPulse.Application.Options;
using NetPulse.Domain.Entities;
using NetPulse.Domain.Interfaces;

namespace NetPulse.Infrastructure.Services;

public class TelegramNotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly TelegramOptions _options;

    public TelegramNotificationService(
        HttpClient httpClient,
        IOptions<TelegramOptions> options
    )
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendAlertAsync(LogEntry log, CancellationToken cancellationToken = default)
    {
        var url =  $"https://api.telegram.org/bot{_options.BotToken}/sendMessage";

        var payload = new
        {
            chat_id = _options.ChatId,
            text = $"NetPulse alarm.\n\nPoziom: `{log.Level}`\nPlik: `{log.FilePath}`\nWiadomość: `{log.Message}`",
            parse_mode = "Markdown"
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}