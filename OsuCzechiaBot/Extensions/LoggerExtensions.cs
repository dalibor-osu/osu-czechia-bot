using OsuCzechiaBot.Helpers.Optionals;

namespace OsuCzechiaBot.Extensions;

public static class LoggerExtensions
{
    public static void LogError(this ILogger logger, Error error)
    {
        logger.LogError("{Message}", error.Message);
    }
}