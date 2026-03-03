using Microsoft.Extensions.Caching.Memory;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using OsuCzechiaBot.Constants;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Handlers;

public class MessageReactionHandler(
    IMemoryCache memoryCache,
    ReactionRoleDatabaseService reactionRoleDatabaseService,
    UserManager userManager) : IMessageReactionAddGatewayHandler, IMessageReactionRemoveGatewayHandler
{
    public async ValueTask HandleAsync(MessageReactionAddEventArgs arg)
    {
        if (arg.MessageId != memoryCache.Get<ulong>(CacheKeys.RoleMessageId))
        {
            return;
        }

        var reactionRole = await reactionRoleDatabaseService.GetByAsync((r) => r.EmojiId == arg.Emoji.Id && r.EmojiName == arg.Emoji.Name);
        if (reactionRole == null)
        {
            return;
        }

        await userManager.AddRole(arg.UserId, reactionRole.RoleId);
    }

    public async ValueTask HandleAsync(MessageReactionRemoveEventArgs arg)
    {
        if (arg.MessageId != memoryCache.Get<ulong>(CacheKeys.RoleMessageId))
        {
            return;
        }
        
        var reactionRole = await reactionRoleDatabaseService.GetByAsync((r) => r.EmojiId == arg.Emoji.Id && r.EmojiName == arg.Emoji.Name);
        if (reactionRole == null)
        {
            return;
        }
        
        await userManager.RemoveRole(arg.UserId, reactionRole.RoleId);
    }
}