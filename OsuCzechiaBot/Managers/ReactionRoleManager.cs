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
    ConfigurationAccessor configurationAccessor)
{
    public async Task<bool> AddAsync(string reactEmoji, Role role)
    {
        if (!EmojiHelper.TryParse(reactEmoji, out var reactEmojiResult))
        {
            return false;
        }

        var reactionRole = new ReactionRole
        {
            EmojiId = reactEmojiResult.Id,
            EmojiName = reactEmojiResult.Name,
            IsAnimated = reactEmojiResult.IsAnimated,
            RoleId = role.Id
        };

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

    public async Task<RestMessage> EnsureRoleMessageAsync()
    {
        var message = await GetRoleMessageAsync();
        if (message is not null)
        {
            return message;
        }

        var reactionRoles = await reactionRoleDatabaseService.ListAsync();
        string messageContent = BuildRoleMessage(reactionRoles);
        message = await restClient.SendMessageAsync(configurationAccessor.Discord.RoleChannelId, messageContent);
        cache.Set(CacheKeys.RoleMessageId, message.Id);
        await EnsureCorrectReactionsAsync(message, reactionRoles);
        return message;
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
        await message.ModifyAsync((options => options.WithContent(BuildRoleMessage(reactionRoles))));
        await EnsureCorrectReactionsAsync(message, reactionRoles);
    }

    private async Task EnsureCorrectReactionsAsync(RestMessage message, IReadOnlyCollection<ReactionRole> reactionRoles)
    {
        var existingReactions = message.Reactions.Where(r => r.Me).ToList();
        var reactionsToRemove =
            existingReactions.Where(r => !reactionRoles.Any(rr => rr.EmojiId == r.Emoji.Id && rr.EmojiName == r.Emoji.Name));
        var reactionsToAdd =
            reactionRoles.Where(rr => !existingReactions.Any(r => r.Emoji.Id == rr.EmojiId && r.Emoji.Name == rr.EmojiName));

        foreach (var reactionRole in reactionsToRemove)
        {
            var properties = new ReactionEmojiProperties(reactionRole.Emoji.Name!, reactionRole.Emoji.Id!.Value);
            await restClient.DeleteAllMessageReactionsForEmojiAsync(configurationAccessor.Discord.RoleChannelId, message.Id, properties);
        }

        foreach (var reactionRole in reactionsToAdd)
        {
            var properties = new ReactionEmojiProperties(reactionRole.EmojiName, reactionRole.EmojiId);
            await restClient.AddMessageReactionAsync(configurationAccessor.Discord.RoleChannelId, message.Id, properties);
        }
    }
    
    private static string BuildRoleMessage(IReadOnlyCollection<ReactionRole> reactionRoles)
    {
        var builder = new StringBuilder(BotMessages.RoleReactionMessage);
        builder.Append('\n');
        foreach (var reactionRole in reactionRoles)
        {
            builder.AppendLine($"{reactionRole.GetEmojiString()} - {reactionRole.GetRoleString()}");
        }
        return builder.ToString();
    }
}