using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Handlers;

public sealed class UserRemoveHandler : IGuildUserRemoveGatewayHandler, IDisposable, IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly ConfigurationAccessor _configurationAccessor;
    private readonly AuthorizedUserDatabaseService _dbService;
    private readonly DiscordLogManager _discordLogManager;
    
    public UserRemoveHandler(IServiceProvider serviceProvider)
    {
        _scope = serviceProvider.CreateAsyncScope();
        _configurationAccessor = _scope.ServiceProvider.GetRequiredService<ConfigurationAccessor>();
        _dbService = _scope.ServiceProvider.GetRequiredService<AuthorizedUserDatabaseService>();
        _discordLogManager = _scope.ServiceProvider.GetRequiredService<DiscordLogManager>();
    }

    public async ValueTask HandleAsync(GuildUserRemoveEventArgs arg)
    {
        if (_configurationAccessor.Discord.GuildId != arg.GuildId)
        {
            return;
        }

        var removedUser = await _dbService.RemoveAsync(arg.User.Id);
        if (removedUser is null)
        {
            await _discordLogManager.LogAsync($"User <@{arg.User.Id}> left.");
            return;
        }

        await _discordLogManager.LogAsync($"User <@{arg.User.Id}> left. They were unlinked and removed from the database.");
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}