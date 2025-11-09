using OsuCzechiaBot.Exceptions;

namespace OsuCzechiaBot.Configuration;

public class DiscordConfiguration
{
    public string Token { get; set; } = string.Empty;
    public ulong GuildId { get; set; }
    public ulong AuthChannelId { get; set; }
    public ulong LogChannelId { get; set; }
    public ulong AuthorizedRoleId { get; set; }
    
    public ulong OneDigitRoleId { get; set; }
    public ulong TwoDigitRoleId { get; set; }
    public ulong ThreeDigitRoleId { get; set; }
    public ulong FourDigitRoleId { get; set; }
    public ulong FiveDigitRoleId { get; set; }
    public ulong SixDigitRoleId { get; set; }
    public ulong SevenDigitRoleId { get; set; }
    public ulong EightDigitRoleId { get; set; }
    public ulong NineDigitRoleId { get; set; }
    public ulong TenDigitRoleId { get; set; }

    public ulong[] AllDigitRoleIds =>
    [
        OneDigitRoleId,
        TwoDigitRoleId,
        ThreeDigitRoleId,
        FourDigitRoleId,
        FiveDigitRoleId,
        SixDigitRoleId,
        SevenDigitRoleId,
        EightDigitRoleId,
        NineDigitRoleId,
        TenDigitRoleId
    ];

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new ConfigurationException("Discord Token is not configured");
        }

        if (GuildId == 0)
        {
            throw new ConfigurationException("Discord GuildId is not configured");
        }

        if (AuthChannelId == 0)
        {
            throw new ConfigurationException("Discord AuthChannelId is not configured");
        }

        if (LogChannelId == 0)
        {
            throw new ConfigurationException("Discord LogChannelId is not configured");
        }

        if (AuthorizedRoleId == 0)
        {
            throw new ConfigurationException("Discord AuthorizedRoleId is not configured");
        }
    }
}