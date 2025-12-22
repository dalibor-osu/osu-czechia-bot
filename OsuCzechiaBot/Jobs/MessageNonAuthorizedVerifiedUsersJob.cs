using NetCord;
using NetCord.Rest;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Jobs.OneTime.Base;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Jobs;

public class MessageNonAuthorizedVerifiedUsersJob(
    ILogger<MessageNonAuthorizedVerifiedUsersJob> logger,
    ConfigurationAccessor configurationAccessor,
    UserManager userManager,
    DiscordLogManager discordLogManager,
    RestClient restClient) : OneTimeJob
{
    public override string Key => nameof(MessageNonAuthorizedVerifiedUsersJob);
    public override DateTimeOffset? RunAt => new DateTimeOffset(2025, 12, 22, 8, 0, 0, TimeSpan.Zero);

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        var allGuildUsers = await restClient.GetGuildUsersAsync(configurationAccessor.Discord.GuildId).ToListAsync(cancellationToken);
        List<ulong> failedUserIds = [];
        foreach (var guildUser in allGuildUsers)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            bool result = await ProcessUser(guildUser, cancellationToken);
            if (!result)
            {
                failedUserIds.Add(guildUser.Id);
            }
        }

        if (failedUserIds.Count == 0)
        {
            await discordLogManager.LogAsync("Finished messaging manually verified users.");
            return;
        }
        
        string failedUsersString = string.Join("\n", failedUserIds.Select(id => $"<@{id}>"));
        await discordLogManager.LogAsync($"Finished messaging manually verified users. Failed messaging these users:\n\n{failedUsersString}");
    }

    private async Task<bool> ProcessUser(GuildUser guildUser, CancellationToken cancellationToken)
    {
        try
        {
            if (!guildUser.RoleIds.Contains(configurationAccessor.Discord.AuthorizedRoleId))
            {
                return true;
            }

            var authorizedUser = await userManager.GetAsync(guildUser.Id);
            if (authorizedUser is not null)
            {
                return true;
            }

            return await userManager.MessageUserAsync(guildUser.Id, Message, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Something went wrong when processing user: {UserId} | {Message}", guildUser.Id, e.Message);
            return false;
        }
    }
    
    private const string Message = 
        """
        Hi! This is an automated message. You recieved it because there is a mismatch between your verified status in osu! Czechia discord server and this bot.
        
        This is most likely caused by a moderator assigning you the "Verified" role manually, without you using the */authorize* command. As a result, the bot cannot access information about your osu! account, which breaks automatic role updates and may affect future features.
        
        To fix this, please run the */authorize* command. This will link your osu! profile to the bot and update your roles accordingly. If you do not complete this step, your verification status may be removed in the future. If you encounter any issues, feel free to contact <@241541462729162752>.
        
        Thank you for your time. With love, Dalibor <3
        """;
}