using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Jobs.OneTime.Base;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Jobs;

public class OneTimeJobRunner(IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = provider.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<OneTimeJobRunner>>();
        var oneTimeJobs = scope.ServiceProvider.GetServices<IOneTimeJob>().ToList();
        if (oneTimeJobs.Count == 0)
        {
            logger.LogInformation("No one time jobs to run.");
            return;
        }

        var dbService = scope.ServiceProvider.GetRequiredService<OneTimeJobLogDatabaseService>();
        var alreadyRunList = await dbService.GetManyAsync(oneTimeJobs.Select(otj => otj.Key).ToList());
        var toRunList = oneTimeJobs.Where(oneTimeJob => alreadyRunList.All(alreadyRunJob => alreadyRunJob.Id != oneTimeJob.Key)).ToList();
        if (toRunList.Count == 0)
        {
            logger.LogInformation("No one time jobs to run.");
            return;
        }
        
        var immediateJobs = toRunList.Where(j => j.RunAt is null).ToList();
        var delayedJobs = toRunList.Where(j => j.RunAt is not null).ToList();

        foreach (var oneTimeJob in immediateJobs)
        {
            await RunJobNow(oneTimeJob, dbService, stoppingToken);
        }

        var plannedJobs = delayedJobs.Select(oneTimeJob => RunJobDelayed(oneTimeJob, dbService, stoppingToken));
        await Task.WhenAll(plannedJobs);
    }

    private static async Task RunJobDelayed(IOneTimeJob oneTimeJob, OneTimeJobLogDatabaseService dbService, CancellationToken stoppingToken)
    {
        var currentTime = DateTimeOffset.UtcNow;
        if (oneTimeJob.RunAt!.Value <= currentTime)
        {
            await RunJobNow(oneTimeJob, dbService, stoppingToken);
            return;
        }
        
        await Task.Delay(oneTimeJob.RunAt!.Value - currentTime, stoppingToken);
        if (stoppingToken.IsCancellationRequested)
        {
            return;
        }
        
        await RunJobNow(oneTimeJob, dbService, stoppingToken);
    }

    private static async Task RunJobNow(IOneTimeJob oneTimeJob, OneTimeJobLogDatabaseService dbService, CancellationToken stoppingToken)
    {
        if (await dbService.ExistsAsync(oneTimeJob.Key))
        {
            return;
        }

        var log = new OneTimeJobLog
        {
            Id = oneTimeJob.Key,
            StartTime = DateTimeOffset.UtcNow
        };

        await oneTimeJob.RunAsync(stoppingToken);
        if (!stoppingToken.IsCancellationRequested)
        {
            await dbService.AddAsync(log);
        }
    }
}