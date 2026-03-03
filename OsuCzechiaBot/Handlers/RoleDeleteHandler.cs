using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Handlers;

public class RoleDeleteHandler(ReactionRoleManager reactionRoleManager, DiscordLogManager discordLogManager) : IRoleDeleteGatewayHandler
{
    public async ValueTask HandleAsync(RoleDeleteEventArgs arg)
    {
        if (await reactionRoleManager.RemoveAsync(arg.RoleId, false))
        {
            await discordLogManager.LogAsync($"Role with ID: {arg.RoleId} has been deleted and was removed from reaction roles");
        }
    }
}