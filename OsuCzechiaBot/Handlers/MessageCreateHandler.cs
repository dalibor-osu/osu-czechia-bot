using System.Collections.Concurrent;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Handlers;

public class MessageCreateHandler : IMessageCreateGatewayHandler
{
    private readonly TimeSpan _window = TimeSpan.FromSeconds(3);
    private const int MaxMessages = 5;

    private readonly ConcurrentDictionary<ulong, Queue<DateTimeOffset>> _userMessages = new();
    private readonly ConcurrentDictionary<ulong, object> _userLocks = new();
    
    private readonly ConfigurationAccessor _configurationAccessor;
    private readonly IServiceProvider _serviceProvider;

    public MessageCreateHandler(ConfigurationAccessor configurationAccessor, IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    {
        _configurationAccessor = configurationAccessor;
        _serviceProvider = serviceProvider;
        _ = ClearAllMessagesWorker(applicationLifetime.ApplicationStopping);
    }

    public async ValueTask HandleAsync(Message arg)
    {
        if (arg.ChannelId != _configurationAccessor.Discord.AuthChannelId || arg.Author.IsBot)
        {
            return;
        }

        if (arg.Author is GuildUser guildUser)
        {
            var roles = guildUser.GetRoles(arg.Guild!).ToList();
            if (roles.Any(r => r.Permissions.HasFlag(Permissions.Administrator)) || arg.Guild?.OwnerId == guildUser.Id)
            {
                return;
            }
        }

        if (ShouldBeTimedOut(arg.Author.Id))
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
            await userManager.TimeOutUserAsync(arg.Author.Id, TimeSpan.FromMinutes(1), "Too many messages in authorization channel. If you'd like to get help, contact anyone from the Mod team.");
        }

        await arg.DeleteAsync();
    }

    private bool ShouldBeTimedOut(ulong discordId)
    {
        var now = DateTimeOffset.UtcNow;
        var queue = _userMessages.GetOrAdd(discordId, _ => new Queue<DateTimeOffset>());
        object userLock = _userLocks.GetOrAdd(discordId, _ => new object());

        lock (userLock)
        {
            while (queue.Count > 0 && now - queue.Peek() > _window)
            {
                queue.Dequeue();
            }

            if (queue.Count >= MaxMessages)
            {
                queue.Clear();
                return true;
            }

            queue.Enqueue(now);
            return false;
        }
    }

    private async Task ClearAllMessagesWorker(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            
            _userLocks.Clear();
            _userMessages.Clear();
            await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
        }
    }
}