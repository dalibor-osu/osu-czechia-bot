using Microsoft.Extensions.Caching.Memory;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Constants;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Handlers;

public sealed class MessageReactionHandler : IMessageReactionAddGatewayHandler, IMessageReactionRemoveGatewayHandler, IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly ReactionRoleDatabaseService _reactionRoleDatabaseService;
    private readonly UserManager _userManager;
    private readonly ConfigurationAccessor _configurationAccessor;
    private readonly IServiceScope _serviceScope;

    public MessageReactionHandler(IServiceProvider serviceProvider)
    {
        _serviceScope = serviceProvider.CreateScope();
        _memoryCache = _serviceScope.ServiceProvider.GetRequiredService<IMemoryCache>();
        _reactionRoleDatabaseService = _serviceScope.ServiceProvider.GetRequiredService<ReactionRoleDatabaseService>();
        _userManager = _serviceScope.ServiceProvider.GetRequiredService<UserManager>();
        _configurationAccessor = serviceProvider.GetRequiredService<ConfigurationAccessor>();
    }

    public async ValueTask HandleAsync(MessageReactionAddEventArgs arg)
    {
        if (arg.UserId == _configurationAccessor.Discord.BotId || arg.User?.IsBot == true)
        {
            return;
        }

        if (arg.MessageId != _memoryCache.Get<ulong>(CacheKeys.RoleMessageId))
        {
            return;
        }

        var reactionRole = await _reactionRoleDatabaseService.GetByMessageReactionEmoji(arg.Emoji);
        if (reactionRole == null)
        {
            return;
        }

        await _userManager.AddRole(arg.UserId, reactionRole.RoleId);
    }

    public async ValueTask HandleAsync(MessageReactionRemoveEventArgs arg)
    {
        if (arg.MessageId != _memoryCache.Get<ulong>(CacheKeys.RoleMessageId))
        {
            return;
        }

        var reactionRole = await _reactionRoleDatabaseService.GetByMessageReactionEmoji(arg.Emoji);
        if (reactionRole == null)
        {
            return;
        }

        await _userManager.RemoveRole(arg.UserId, reactionRole.RoleId);
    }

    public void Dispose() => _serviceScope.Dispose();
}