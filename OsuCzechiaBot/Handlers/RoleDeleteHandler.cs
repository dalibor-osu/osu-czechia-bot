using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Handlers;

public sealed class RoleDeleteHandler : IRoleDeleteGatewayHandler, IDisposable
{
    private readonly ReactionRoleManager _reactionRoleManager;
    private readonly DiscordLogManager _discordLogManager;
    private readonly IServiceScope _scope;

    public RoleDeleteHandler(IServiceProvider provider)
    {
        _scope = provider.CreateScope();
        _reactionRoleManager = _scope.ServiceProvider.GetRequiredService<ReactionRoleManager>();
        _discordLogManager = _scope.ServiceProvider.GetRequiredService<DiscordLogManager>();
    }

    public async ValueTask HandleAsync(RoleDeleteEventArgs arg)
    {
        if (await _reactionRoleManager.RemoveAsync(arg.RoleId, false))
        {
            await _discordLogManager.LogAsync($"Role with ID: {arg.RoleId} has been deleted and was removed from reaction roles");
        }
    }

    public void Dispose() => _scope.Dispose();
}