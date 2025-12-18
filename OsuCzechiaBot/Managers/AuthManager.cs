using NetCord.Rest;
using OsuCzechiaBot.Clients;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Constants;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Extensions;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Managers;

public class AuthManager(
    ILogger<AuthManager> logger,
    RestClient restClient,
    OsuHttpClient osuHttpClient,
    AuthorizedUserDatabaseService authorizedUserDatabaseService,
    UserManager userManager,
    ConfigurationAccessor configurationAccessor)
{
    public async Task<string> AuthorizeUserAsync(ulong discordId, string code, CancellationToken cancellationToken = default)
    {
        var existingUser = await authorizedUserDatabaseService.GetByDiscordIdAsync(discordId);
        if (existingUser != null)
        {
            return HtmlResponses.AuthAlreadyAuthorized;
        }

        var tokenResponse = await osuHttpClient.GetTokenFromCode(code);
        if (tokenResponse == null)
        {
            return HtmlResponses.AuthFailed;
        }

        var authorizedUser = new AuthorizedUser
        {
            Id = discordId,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
        };

        var osuUserData = await osuHttpClient.GetUserData(authorizedUser);
        if (osuUserData == null || osuUserData.Id < 1)
        {
            return HtmlResponses.AuthFailed;
        }

        authorizedUser.OsuId = osuUserData.Id;
        authorizedUser.CountryCode = osuUserData.CountryCode;

        await userManager.UpdateUserRoles(authorizedUser, true, osuUserData, cancellationToken);

        try
        {
            var user = await restClient.GetGuildUserAsync(configurationAccessor.Discord.GuildId, discordId, cancellationToken: cancellationToken);
            if (!string.IsNullOrWhiteSpace(user.Nickname))
            {
                if (!osuUserData.Username.Equals(user.Nickname, StringComparison.OrdinalIgnoreCase))
                {
                    await user.ModifyAsync(guidUser => guidUser.WithNickname($"{user.Nickname} ({osuUserData.Username})"), cancellationToken: cancellationToken);
                }
            }
            else if (!string.IsNullOrWhiteSpace(user.GlobalName))
            {
                if (!osuUserData.Username.Equals(user.GlobalName, StringComparison.OrdinalIgnoreCase))
                {
                    await user.ModifyAsync(guidUser => guidUser.WithNickname($"{user.GlobalName} ({osuUserData.Username})"), cancellationToken: cancellationToken);
                }
            }
            else
            {
                if (!osuUserData.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase))
                {
                    await user.ModifyAsync(guidUser => guidUser.WithNickname($"{user.Username} ({osuUserData.Username})"), cancellationToken: cancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Couldn't modify user's nickname");
        }

        await authorizedUserDatabaseService.AddOrUpdateAsync(authorizedUser);
        await restClient.SendMessageAsync(configurationAccessor.Discord.LogChannelId,
            $"Successfully authorized <@{discordId}> with {authorizedUser.GetMarkdownOsuProfileLink()}!",
            cancellationToken: cancellationToken);

        return HtmlResponses.AuthSuccess;
    }
}