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

    public override Task RunAsync(CancellationToken cancellationToken)
    {
        
    }
}