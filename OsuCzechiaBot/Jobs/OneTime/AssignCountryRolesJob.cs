using System.Diagnostics;
using System.Text;
using NetCord.Rest;
using OsuCzechiaBot.Clients;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Jobs.OneTime.Base;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Jobs.OneTime;

public class AssignCountryRolesJob(
    ILogger<AssignCountryRolesJob> logger,
    AuthorizedUserDatabaseService authorizedUserDatabaseService,
    OsuHttpClient osuHttpClient,
    UserManager userManager,
    RestClient restClient,
    ConfigurationAccessor configurationAccessor,
    DiscordLogManager discordLogManager) : OneTimeJob
{
    public override string Key => nameof(AssignCountryRolesJob);
    private const int MillisecondsPerUser = 10 * 1000;

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        var users = await authorizedUserDatabaseService.ListAsync();
        var stopwatch = new Stopwatch();
        var failedIds = new List<(ulong id, int osuId)>(users.Count);

        foreach (var user in users)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await discordLogManager.Log($"{nameof(AssignCountryRolesJob)} was canceled and will run again.",
                    LogLevel.Warning);
                return;
            }

            stopwatch.Restart();
            if (!user.Authorized)
            {
                continue;
            }

            try
            {
                var data = await osuHttpClient.GetUserData(user);
                if (data is null)
                {
                    logger.LogError("Failed to get data for user: {UserId}", user.Id);
                    failedIds.Add((user.Id, user.OsuId));
                    continue;
                }

                await userManager.UpdateUserRoles(user, false, data, cancellationToken);
            }
            catch (Exception e)
            {
                failedIds.Add((user.Id, user.OsuId));
                logger.LogError("Failed to update country role for user: {UserId}. Message: {Message}", user.Id,
                    e.Message);
            }
            finally
            {
                await WaitForTimeout(stopwatch.ElapsedMilliseconds, cancellationToken);
            }
        }

        var stringBuilder = new StringBuilder("Finished assigning country roles.");
        if (failedIds.Count != 0)
        {
            stringBuilder.Append($" Failed to update for {failedIds.Count} users.");
        }

        await discordLogManager.Log(stringBuilder.ToString());
    }

    private async Task WaitForTimeout(long elapsedMilliseconds, CancellationToken cancellationToken)
    {
        long remainingMilliseconds = MillisecondsPerUser - elapsedMilliseconds;
        if (remainingMilliseconds <= 0)
        {
            return;
        }

        await Task.Delay(TimeSpan.FromMilliseconds(remainingMilliseconds), cancellationToken);
    }
}