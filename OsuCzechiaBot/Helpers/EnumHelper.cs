namespace OsuCzechiaBot.Helpers;

public static class EnumHelper
{
    public static T ParseOrDefault<T>(string? value, T defaultValue) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
    }
}