using NetCord;
using NetCord.Rest;
using OsuCzechiaBot.Clients;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Extensions;
using OsuCzechiaBot.Helpers.Optionals;
using OsuCzechiaBot.Models;
using OsuCzechiaBot.Models.OsuApi;

namespace OsuCzechiaBot.Managers;

public class UserManager(
    ILogger<UserManager> logger,
    ConfigurationAccessor configurationAccessor,
    RestClient restClient,
    OsuHttpClient osuHttpClient,
    AuthorizedUserDatabaseService dbService,
    DiscordLogManager discordLogManager)
{
    public async Task<AuthorizedUser?> GetAsync(ulong discordId) => await dbService.GetAsync(discordId);

    public async Task<bool> MessageUserAsync(ulong discordId, string message, CancellationToken cancellationToken = default)
    {
        var guildUser = await GetGuildUserAsync(discordId, cancellationToken: cancellationToken);
        if (guildUser is null)
        {
            return false;
        }

        try
        {
            var dmChannel = await guildUser.GetDMChannelAsync(cancellationToken: cancellationToken);
            await dmChannel.SendMessageAsync(new MessageProperties
            {
                Content = message
            }, cancellationToken: cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Something went wrong when DMing user: {UserId}", discordId);
            return false;
        }
    }

    public async Task UpdateUserRoles(AuthorizedUser user, bool authorize,
        OsuUserExtendedWithOptionalData? userData = null,
        CancellationToken cancellationToken = default)
    {
        userData ??= await osuHttpClient.GetUserData(user);
        if (userData == null)
        {
            logger.LogError("Failed to get user data");
            return;
        }

        if (!await RemoveAllUserRankRoles(user, cancellationToken))
        {
            return;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (authorize)
        {
            await restClient.AddGuildUserRoleAsync(configurationAccessor.Discord.GuildId, user.Id,
                configurationAccessor.Discord.AuthorizedRoleId,
                cancellationToken: cancellationToken);
        }

        if (userData.IsRestricted)
        {
            return;
        }

        ulong newRankRoleId = GetRankRoleIdForUser(userData);
        if (newRankRoleId != 0)
        {
            await restClient.AddGuildUserRoleAsync(configurationAccessor.Discord.GuildId, user.Id, newRankRoleId,
                cancellationToken: cancellationToken);
        }


        ulong newCountryRankRoleId = GetCountryRankRoleIdForUser(userData);
        if (newCountryRankRoleId != 0 && userData.IsCzech())
        {
            await restClient.AddGuildUserRoleAsync(configurationAccessor.Discord.GuildId, user.Id, newCountryRankRoleId,
                cancellationToken: cancellationToken);
        }
    }

    public async Task<bool> UnlinkUserAsync(ulong discordId, ApplicationCommandInteraction? interaction = null,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await dbService.RemoveAsync(discordId);
        if (existingUser is null)
        {
            if (interaction is not null)
            {
                await interaction.SendFollowupMessageAsync(new InteractionMessageProperties
                {
                    Content =
                        "Couldn't unlink your osu! profile, because you are not authorized yet. Please use the */authorize* command.",
                    Flags = MessageFlags.Ephemeral
                }, cancellationToken: cancellationToken);
            }

            return false;
        }

        if (!await RemoveAllUserRankRoles(existingUser, cancellationToken))
        {
            return false;
        }

        await restClient.RemoveGuildUserRoleAsync(configurationAccessor.Discord.GuildId, discordId,
            configurationAccessor.Discord.AuthorizedRoleId,
            cancellationToken: cancellationToken);
        return true;
    }

    public async Task TimeOutUserAsync(ulong discordId, TimeSpan duration, string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var guildUser = await GetGuildUserAsync(discordId, cancellationToken: cancellationToken);
        if (guildUser is null)
        {
            return;
        }

        await guildUser.ModifyAsync((options => options.WithTimeOutUntil(DateTimeOffset.Now + duration)),
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(reason))
        {
            return;
        }

        await MessageUserAsync(discordId, reason, cancellationToken);
    }

    public async Task<bool> RefreshTokenAsync(ulong discordId, CancellationToken cancellationToken = default)
    {
        var user = await dbService.GetAsync(discordId);
        if (user == null)
        {
            logger.LogError("Failed to get user data: {UserId}", discordId);
            return false;
        }

        var currentTime = DateTimeOffset.UtcNow;
        var tokenResponseResult = await osuHttpClient.GetTokenFromRefreshToken(user.RefreshToken);
        if (!tokenResponseResult.Success)
        {
            logger.LogError("Failed to get new tokens for user: {UserId} | {Message}", discordId, tokenResponseResult.Error.Message);
            if (tokenResponseResult.Error.ErrorType != ErrorType.Forbidden && !(user.Expires <= currentTime))
            {
                return false;
            }

            user.Authorized = false;
            user.AccessToken = string.Empty;
            user.RefreshToken = string.Empty;
            user.Expires = null;
            await dbService.UpdateAsync(user);

            await discordLogManager.LogAsync(
                $"Failed to update token for <@{user.Id}> ({user.GetMarkdownOsuProfileLink()})! This user is now marked as not authorized.",
                LogLevel.Error,
                cancellationToken);

            return false;
        }

        var tokenResponse = tokenResponseResult.Value;

        user.RefreshToken = tokenResponse.RefreshToken;
        user.AccessToken = tokenResponse.AccessToken;
        user.Expires = currentTime.AddSeconds(tokenResponse.ExpiresIn);
        user.Authorized = true;

        var result = await dbService.UpdateAsync(user);
        if (result is null)
        {
            logger.LogError("Failed to update user data: {UserId}", discordId);
            return false;
        }

        return true;
    }

    public async Task<bool> RenameUserAsync(ulong discordId, string newName,
        CancellationToken cancellationToken = default)
    {
        var user = await GetGuildUserAsync(discordId, cancellationToken: cancellationToken);
        if (user is null)
        {
            return false;
        }

        try
        {
            await user.ModifyAsync(u => u.WithNickname(newName), cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Something went wrong when DMing user: {UserId}", discordId);
            return false;
        }

        return true;
    }

    public async Task<GuildUser?> GetGuildUserAsync(ulong discordId, bool deleteOnFail = false, CancellationToken cancellationToken = default)
    {
        GuildUser? guildUser = null;
        try
        {
            guildUser = await restClient.GetGuildUserAsync(configurationAccessor.Discord.GuildId, discordId,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get guild user: {Message}", e.Message);
        }

        if (guildUser is not null || !deleteOnFail)
        {
            return guildUser;
        }
        
        logger.LogWarning("User {DiscordId} will be removed, because they were not found in the guild", discordId);
        await dbService.RemoveAsync(discordId);
        return null;
    }

    private async Task<bool> RemoveAllUserRankRoles(AuthorizedUser user, CancellationToken cancellationToken = default)
    {
        var guildUser = await GetGuildUserAsync(user.Id, cancellationToken: cancellationToken);
        if (guildUser is null)
        {
            return false;
        }

        foreach (ulong roleId in configurationAccessor.Discord.AllDigitRoleIds.Concat(configurationAccessor.Discord
                     .AllCountryDigitRoleIds))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (!guildUser.RoleIds.Contains(roleId))
            {
                continue;
            }

            await guildUser.RemoveRoleAsync(roleId, cancellationToken: cancellationToken);
        }

        return true;
    }

    private ulong GetRankRoleIdForUser(OsuUserExtendedWithOptionalData userData)
    {
        ulong rank = userData.GetMainGlobalRank();
        if (rank == 0)
        {
            return 0;
        }

        return rank switch
        {
            < 10 => configurationAccessor.Discord.OneDigitRoleId,
            < 100 => configurationAccessor.Discord.TwoDigitRoleId,
            < 1_000 => configurationAccessor.Discord.ThreeDigitRoleId,
            < 10_000 => configurationAccessor.Discord.FourDigitRoleId,
            < 100_000 => configurationAccessor.Discord.FiveDigitRoleId,
            < 1_000_000 => configurationAccessor.Discord.SixDigitRoleId,
            < 10_000_000 => configurationAccessor.Discord.SevenDigitRoleId,
            < 100_000_000 => configurationAccessor.Discord.EightDigitRoleId,
            < 1_000_000_000 => configurationAccessor.Discord.NineDigitRoleId,
            < 10_000_000_000 => configurationAccessor.Discord.TenDigitRoleId,
            _ => 0
        };
    }

    public ulong GetCountryRankRoleIdForUser(OsuUserExtendedWithOptionalData userData)
    {
        ulong rank = userData.GetMainCountryGlobalRank();
        if (rank == 0)
        {
            return 0;
        }

        return rank switch
        {
            < 2 => configurationAccessor.Discord.TopOneRoleId,
            <= 10 => configurationAccessor.Discord.TopTenRoleId,
            <= 50 => configurationAccessor.Discord.TopFiftyRoleId,
            <= 100 => configurationAccessor.Discord.TopHundredRoleId,
            _ => 0
        };
    }
}