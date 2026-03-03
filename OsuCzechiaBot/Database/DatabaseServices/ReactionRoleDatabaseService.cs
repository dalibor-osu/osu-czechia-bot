using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Database.DatabaseServices;

public class ReactionRoleDatabaseService(OsuCzechiaBotDatabaseContext dbContext)
    : DatabaseServiceBase<ReactionRole, long>(dbContext, dbContext.ReactionRoles);