using System.Text;
using Microsoft.Extensions.Caching.Memory;
using NetCord;
using NetCord.Rest;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Constants;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Helpers;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Managers;

public class ReactionRoleManager(
    ReactionRoleDatabaseService reactionRoleDatabaseService,
    UserManager userManager,
    IMemoryCache cache,
    ILogger<ReactionRoleManager> logger,
    RestClient restClient,
    ConfigurationAccessor configurationAccessor,
    ApplicationSettingDatabaseService applicationSettingDatabaseService)
{
    public async Task<bool> AddAsync(string reactEmoji, Role role, string? description)
    {
        if (!EmojiHelper.TryParse(reactEmoji, out var reactEmojiResult))
        {
            return false;
        }

        var reactionRole = reactEmojiResult.ToReactionRole(role);
        reactionRole.Description = description;
        await reactionRoleDatabaseService.AddAsync(reactionRole);
        await UpdateRoleMessageAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(ulong roleId, bool removeFromUsers)
    {
        var reactionRole = await reactionRoleDatabaseService.GetByAsync(r => r.RoleId == roleId);
        if (reactionRole == null)
        {
            return false;
        }

        await reactionRoleDatabaseService.RemoveAsync(reactionRole.Id);
        await UpdateRoleMessageAsync();

        if (removeFromUsers)
        {
            await userManager.RemoveRoleFromAllUsers(roleId);
        }

        return true;
    }

    public async Task<RestMessage?> GetRoleMessageAsync()
    {
        RestMessage? message = null;
        if (cache.TryGetValue(CacheKeys.RoleMessageId, out ulong messageId))
        {
            try
            {
                message = await restClient.GetMessageAsync(configurationAccessor.Discord.RoleChannelId, messageId);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get exact role message: {Id}", messageId);
            }
        }

        if (message is not null)
        {
            return message;
        }

        try
        {
            message = await restClient.GetMessagesAsync(configurationAccessor.Discord.RoleChannelId)
                .FirstOrDefaultAsync(m => m.Author.Id == configurationAccessor.Discord.BotId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get role message");
            return null;
        }

        if (message != null)
        {
            cache.Set(CacheKeys.RoleMessageId, message.Id);
        }
        else
        {
            cache.Remove(CacheKeys.RoleMessageId);
        }

        return message;
    }

    public async Task EnsureRoleMessageAsync()
    {
        var message = await GetRoleMessageAsync();
        if (message is not null)
        {
            return;
        }

        var reactionRoles = await reactionRoleDatabaseService.ListAsync();
        string messageContent = await BuildRoleMessageAsync(reactionRoles);
        if (messageContent.Length < 1)
        {
            return;
        }

        message = await restClient.SendMessageAsync(configurationAccessor.Discord.RoleChannelId, messageContent);
        cache.Set(CacheKeys.RoleMessageId, message.Id);
        await EnsureCorrectReactionsAsync(message, reactionRoles);
    }

    public async Task UpdateRoleMessageAsync()
    {
        var message = await GetRoleMessageAsync();
        if (message is null)
        {
            await EnsureRoleMessageAsync();
            return;
        }

        var reactionRoles = await reactionRoleDatabaseService.ListAsync();
        string content = await BuildRoleMessageAsync(reactionRoles);
        if (content.Length < 1)
        {
            return;
        }

        await message.ModifyAsync((options => options.WithContent(content)));
        await EnsureCorrectReactionsAsync(message, reactionRoles);
    }

    public string GetReactionRoleMessageLink()
    {
        return
            $"https://discord.com/channels/{configurationAccessor.Discord.GuildId}/{configurationAccessor.Discord.RoleChannelId}/{cache.Get<ulong>(CacheKeys.RoleMessageId)}";
    }

    public async Task<string> GetRoleMessageContent()
    {
        var reactionRoles = await reactionRoleDatabaseService.ListAsync();
        return await BuildRoleMessageAsync(reactionRoles);
    }

    private async Task EnsureCorrectReactionsAsync(RestMessage message, IReadOnlyCollection<ReactionRole> reactionRoles)
    {
        var existingReactions = message.Reactions.Where(r => r.Me).ToList();
        var reactionsToRemove =
            existingReactions.Where(reaction => !reactionRoles.Any(reactionRole =>
                (reaction.Emoji.Id != null && reactionRole.EmojiId == reaction.Emoji.Id && reactionRole.EmojiName == reaction.Emoji.Name) ||
                (!string.IsNullOrWhiteSpace(reaction.Emoji.Name) && reaction.Emoji.Name == reactionRole.UnicodeValue)));
        var reactionsToAdd =
            reactionRoles.Where(reactionRole => !existingReactions.Any(reaction =>
                (reaction.Emoji.Id != null && reaction.Emoji.Id == reactionRole.EmojiId && reaction.Emoji.Name == reactionRole.EmojiName) ||
                (!string.IsNullOrWhiteSpace(reaction.Emoji.Name) && reaction.Emoji.Name == reactionRole.UnicodeValue)));

        foreach (var reaction in reactionsToRemove)
        {
            var properties = reaction.Emoji.Id is null
                ? new ReactionEmojiProperties(reaction.Emoji.Name!)
                : new ReactionEmojiProperties(reaction.Emoji.Name!, reaction.Emoji.Id!.Value);
            await restClient.DeleteAllMessageReactionsForEmojiAsync(configurationAccessor.Discord.RoleChannelId, message.Id, properties);
        }

        foreach (var reactionRole in reactionsToAdd)
        {
            await restClient.AddMessageReactionAsync(configurationAccessor.Discord.RoleChannelId, message.Id,
                reactionRole.ToEmojiProperties());
        }
    }

    private async Task<string> BuildRoleMessageAsync(IReadOnlyCollection<ReactionRole> reactionRoles)
    {
        string message = await applicationSettingDatabaseService.GetOrEmptyString(SettingsKeys.ReactionRoleMessageKey);
        var builder = new StringBuilder(message.Trim());
        if (builder.Length > 0)
        {
            builder.Append('\n');
        }

        foreach (var reactionRole in reactionRoles)
        {
            builder.Append($"{reactionRole.GetEmojiString()} {reactionRole.GetRoleString()}");
            if (!string.IsNullOrWhiteSpace(reactionRole.Description))
            {
                builder.Append($" - {reactionRole.Description}");
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }
}