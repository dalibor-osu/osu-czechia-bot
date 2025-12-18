using System.Diagnostics;
using System.Text;
using OsuCzechiaBot.Clients;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Jobs.OneTime.Base;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Jobs.OneTime;

public class AssignCountryRolesJob(
    AuthorizedUserDatabaseService authorizedUserDatabaseService,
    OsuHttpClient osuHttpClient,
    UserManager userManager,
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

            var data = await osuHttpClient.GetUserData(user);
            if (data is null)
            {
                failedIds.Add((user.Id, user.OsuId));
                await WaitForTimeout(stopwatch.ElapsedMilliseconds, cancellationToken);
                continue;
            }

            await userManager.UpdateUserRoles(user, false, data, cancellationToken);
        }

        var stringBuilder = new StringBuilder($"Finished assigning country roles.");
        if (failedIds.Count != 0)
        {
            stringBuilder.Append(" Failed to update for these users:");
            foreach ((ulong id, int osuId) in failedIds)
            {
                stringBuilder.AppendLine($"\tID: {id}, OSU_ID: {osuId}");
            }
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