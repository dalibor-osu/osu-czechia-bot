using Microsoft.EntityFrameworkCore;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Database.DatabaseServices;

public class AuthorizedUserDatabaseService(OsuCzechiaBotDatabaseContext dbContext) : DatabaseServiceBase<AuthorizedUser, ulong>(dbContext, dbContext.AuthorizedUsers)
{
    public async Task<IReadOnlyCollection<ulong>> ListExpiringIdsAsync()
    {
        var threshold = DateTimeOffset.UtcNow.AddHours(1);
        return await DbSet.Where(u => u.Expires <= threshold && u.Authorized).Select(u => u.Id).ToListAsync();
    }

    public async Task<AuthorizedUser?> GetByDiscordIdAsync(ulong discordId)
    {
        return await GetByAsync(u => u.Id == discordId);
    }

    public async Task<AuthorizedUser?> GetByOsuId(int osuId)
    {
        return await GetByAsync(u => u.OsuId == osuId);
    }

    public override async Task<AuthorizedUser?> UpdateAsync(AuthorizedUser user)
    {
        var existingUser = await DbSet.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existingUser is null)
        {
            return null;
        }
        
        existingUser.RefreshToken = user.RefreshToken;
        existingUser.AccessToken = user.AccessToken;
        existingUser.Expires = user.Expires;
        existingUser.Authorized = user.Authorized;
        existingUser.CountryCode = user.CountryCode;
        existingUser.OsuId = user.OsuId;
        
        await Context.SaveChangesAsync();
        return existingUser;
    }
}