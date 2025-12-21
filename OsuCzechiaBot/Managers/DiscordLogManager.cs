using NetCord.Rest;
using OsuCzechiaBot.Configuration;

namespace OsuCzechiaBot.Managers;

public class DiscordLogManager(
    ILogger<DiscordLogManager> logger,
    RestClient restClient,
    ConfigurationAccessor configurationAccessor)
{
    public async Task LogAsync(string message, LogLevel logLevel = LogLevel.Information, CancellationToken cancellationToken = default)
    {
        try
        {
            await restClient.SendMessageAsync(configurationAccessor.Discord.AdminChannelId, message, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to log to Discord admin channel");
        }
    }
}