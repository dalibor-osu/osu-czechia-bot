using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Jobs;

public class StartupJob(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;
        var reactionRoleManager = provider.GetRequiredService<ReactionRoleManager>();

        await InitializeRoleMessageInfo(reactionRoleManager);
    }

    private static async Task InitializeRoleMessageInfo(ReactionRoleManager reactionRoleManager)
    {
        await reactionRoleManager.UpdateRoleMessageAsync();
    }
}