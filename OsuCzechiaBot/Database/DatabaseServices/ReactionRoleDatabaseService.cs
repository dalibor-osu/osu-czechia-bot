using NetCord;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Database.DatabaseServices;

public class ReactionRoleDatabaseService(OsuCzechiaBotDatabaseContext dbContext)
    : DatabaseServiceBase<ReactionRole, long>(dbContext, dbContext.ReactionRoles)
{
    public async Task<ReactionRole?> GetByMessageReactionEmoji(MessageReactionEmoji emoji)
    {
        return await GetByAsync(reactionRole =>
            (!reactionRole.IsUnicode && reactionRole.EmojiId == emoji.Id && reactionRole.EmojiName == emoji.Name) ||
                   (reactionRole.IsUnicode && reactionRole.UnicodeValue == emoji.Name)
        );
    }
}