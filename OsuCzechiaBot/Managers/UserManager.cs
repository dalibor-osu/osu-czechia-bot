using NetCord;
using NetCord.Rest;
using OsuCzechiaBot.Clients;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Extensions;
using OsuCzechiaBot.Models;
using OsuCzechiaBot.Models.OsuApi;

namespace OsuCzechiaBot.Managers;

public class UserManager(
    ILogger<UserManager> logger,
    ConfigurationAccessor configurationAccessor,
    RestClient restClient,
    OsuHttpClient osuHttpClient,
    AuthorizedUserDatabaseService dbService)
{
    public async Task<AuthorizedUser?> GetAsync(ulong discordId) => await dbService.GetByDiscordIdAsync(discordId);

    public async Task MessageUserAsync(ulong discordId, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var guildUser = await restClient.GetGuildUserAsync(configurationAccessor.Discord.GuildId, discordId, cancellationToken: cancellationToken);
            var dmChannel = await guildUser.GetDMChannelAsync(cancellationToken: cancellationToken);
            await dmChannel.SendMessageAsync(new MessageProperties
            {
                Content = message
            }, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Something went wrong when DMing user: {UserId}", discordId);
        }
    }
    
    public async Task UpdateUserRoles(AuthorizedUser user, OsuUserExtendedWithOptionalData? userData = null,
        CancellationToken cancellationToken = default)
    {
        userData ??= await osuHttpClient.GetUserData(user);
        if (userData == null)
        {
            logger.LogError("Failed to get user data");
            return;
        }

        await RemoveAllUserRankRoles(user, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        ulong newRankRoleId = GetRankRoleIdForUser(userData);
        if (newRankRoleId != 0)
        {
            await restClient.AddGuildUserRoleAsync(configurationAccessor.Discord.GuildId, user.DiscordId, newRankRoleId,
                cancellationToken: cancellationToken);
        }

        await restClient.AddGuildUserRoleAsync(configurationAccessor.Discord.GuildId, user.DiscordId, configurationAccessor.Discord.AuthorizedRoleId,
            cancellationToken: cancellationToken);
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
                    Content = "Couldn't unlink your osu! profile, because you are not authorized yet. Please use the */authorize* command.",
                    Flags = MessageFlags.Ephemeral
                }, cancellationToken: cancellationToken);
            }

            return false;
        }

        await RemoveAllUserRankRoles(existingUser, cancellationToken);
        await restClient.RemoveGuildUserRoleAsync(configurationAccessor.Discord.GuildId, discordId, configurationAccessor.Discord.AuthorizedRoleId,
            cancellationToken: cancellationToken);
        return true;
    }

    public async Task TimeOutUserAsync(ulong discordId, TimeSpan duration, string? reason = null, CancellationToken cancellationToken = default)
    {
        var guildUser = await restClient.GetGuildUserAsync(configurationAccessor.Discord.GuildId, discordId, cancellationToken: cancellationToken);
        await guildUser.ModifyAsync((options => options.WithTimeOutUntil(DateTimeOffset.Now + duration)), cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(reason))
        {
            return;
        }

        await MessageUserAsync(discordId, reason, cancellationToken);
    }

    private async Task RemoveAllUserRankRoles(AuthorizedUser user, CancellationToken cancellationToken = default)
    {
        var guildUser =
            await restClient.GetGuildUserAsync(configurationAccessor.Discord.GuildId, user.DiscordId, cancellationToken: cancellationToken);
        foreach (ulong roleId in configurationAccessor.Discord.AllDigitRoleIds)
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
}