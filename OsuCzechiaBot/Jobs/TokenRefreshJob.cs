using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Jobs;

public class TokenRefreshJob(IServiceProvider serviceProvider) : BackgroundService
{
    private const int DelaySeconds = 60 * 60;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var currentTime = DateTimeOffset.UtcNow;
            var nextStartTime = currentTime.AddSeconds(DelaySeconds);
            await using var scope = serviceProvider.CreateAsyncScope();
            var provider = scope.ServiceProvider;

            var dbService = provider.GetRequiredService<AuthorizedUserDatabaseService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
            var logger = provider.GetRequiredService<ILogger<TokenRefreshJob>>();
        
            logger.LogInformation("Starting token refresh job");
            var userIds = await dbService.ListExpiringIdsAsync();
            int successCount = 0;
            foreach (ulong userId in userIds)
            {
                logger.LogInformation("Refreshing token for user: {UserId}", userId);
                bool success = await userManager.RefreshTokenAsync(userId, stoppingToken);
                if (success)
                {
                    successCount++;
                }
                else
                {
                    logger.LogError("Failed to refresh token for user: {UserId}", userId);
                }
            }
        
            logger.LogInformation("Updated {Count} user tokens", successCount);
            await Task.Delay(nextStartTime - currentTime, stoppingToken);
        }
    }
}