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
        if (logLevel == LogLevel.None)
        {
            return;
        }

        try
        {
            await restClient.SendMessageAsync(configurationAccessor.Discord.AdminChannelId, UpdateMessageText(message, logLevel),
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to log to Discord admin channel");
        }
    }

    private string UpdateMessageText(string message, LogLevel logLevel)
    {
        string prefix = logLevel switch
        {
            LogLevel.Trace => "**[Trace]** ",
            LogLevel.Debug => "**[Debug]** ",
            LogLevel.Information => "**[Info]** ",
            LogLevel.Warning => "**[Warning]** ",
            LogLevel.Error => "**[Error]** ",
            LogLevel.Critical => "**[Critical]** ",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };

        string postfix = logLevel is LogLevel.Error or LogLevel.Critical && configurationAccessor.Discord.BotAdminUserId != 0m
            ? $" <@{configurationAccessor.Discord.BotAdminUserId}>"
            : string.Empty;
        return $"{prefix}{message}{postfix}";
    }
}