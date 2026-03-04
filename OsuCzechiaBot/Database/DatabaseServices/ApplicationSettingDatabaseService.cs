using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Database.DatabaseServices;

public class ApplicationSettingDatabaseService(OsuCzechiaBotDatabaseContext dbContext)
    : DatabaseServiceBase<ApplicationSetting, string>(dbContext, dbContext.ApplicationSettings)
{
    public async Task<string> GetOrEmptyString(string key)
    {
        return (await GetAsync(key))?.Value ?? string.Empty;
    }
}