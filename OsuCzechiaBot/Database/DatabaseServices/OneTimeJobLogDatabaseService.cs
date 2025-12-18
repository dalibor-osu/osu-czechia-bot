using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Database.DatabaseServices;

public class OneTimeJobLogDatabaseService(OsuCzechiaBotDatabaseContext dbContext)
    : DatabaseServiceBase<OneTimeJobLog, string>(dbContext, dbContext.OneTimeJobLogs);