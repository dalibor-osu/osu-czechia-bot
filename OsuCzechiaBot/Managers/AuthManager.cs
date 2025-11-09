using NetCord.Rest;
using OsuCzechiaBot.Clients;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Constants;
using OsuCzechiaBot.Database.DatabaseServices;
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
    public async Task<IResult> AuthorizeUserAsync(ulong discordId, string code, CancellationToken cancellationToken = default)
    {
        var existingUser = await authorizedUserDatabaseService.GetByDiscordIdAsync(discordId);
        if (existingUser != null)
        {
            return Results.Content(HtmlResponses.AuthAlreadyAuthorized, MediaTypes.Html);
        }

        var tokenResponse = await osuHttpClient.GetTokenFromCode(code);
        if (tokenResponse == null)
        {
            return Results.Content(HtmlResponses.AuthSomethingWentWrong, MediaTypes.Html);
        }

        var authorizedUser = new AuthorizedUser
        {
            DiscordId = discordId,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
        };

        var osuUserData = await osuHttpClient.GetUserData(authorizedUser);
        if (osuUserData == null)
        {
            return Results.Content(HtmlResponses.AuthSomethingWentWrong, MediaTypes.Html);
        }

        authorizedUser.OsuId = osuUserData.Id;
        authorizedUser.CountryCode = osuUserData.CountryCode;

        await userManager.UpdateUserRoles(authorizedUser, osuUserData, cancellationToken);

        try
        {
            var user = await restClient.GetGuildUserAsync(configurationAccessor.Discord.GuildId, discordId, cancellationToken: cancellationToken);
            await user.ModifyAsync(u => u.WithNickname(osuUserData.Username), cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Couldn't modify user's nickname");
        }

        await authorizedUserDatabaseService.AddOrUpdateAsync(authorizedUser);
        await restClient.SendMessageAsync(configurationAccessor.Discord.LogChannelId,
            $"Successfully authorized <@{discordId}> with [osu profile](https://osu.ppy.sh/users/{authorizedUser.OsuId})!",
            cancellationToken: cancellationToken);

        return Results.Content(HtmlResponses.AuthSuccess, MediaTypes.Html);
    }
}