using Microsoft.EntityFrameworkCore;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Database.DatabaseServices;

public class AuthorizedUserDatabaseService(OsuCzechiaBotDatabaseContext dbContext)
{
    public async Task<AuthorizedUser> AddAsync(AuthorizedUser user)
    {
        dbContext.AuthorizedUsers.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<AuthorizedUser> AddOrUpdateAsync(AuthorizedUser user)
    {
        if (!await dbContext.AuthorizedUsers.AnyAsync(u => u.DiscordId == user.DiscordId))
        {
            dbContext.AuthorizedUsers.Add(user);
        }
        else
        {
            dbContext.AuthorizedUsers.Update(user);
        }
        
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<AuthorizedUser?> GetByDiscordIdAsync(ulong discordId)
    {
        return await dbContext.AuthorizedUsers.FirstOrDefaultAsync(u => u.DiscordId == discordId);
    }

    public async Task<AuthorizedUser?> GetByOsuId(int osuId)
    {
        return await dbContext.AuthorizedUsers.FirstOrDefaultAsync(u => u.OsuId == osuId);
    }

    public async Task<AuthorizedUser?> UpdateOsuIdAsync(ulong discordId, int osuId)
    {
        var user = await dbContext.AuthorizedUsers.FirstOrDefaultAsync(u => u.DiscordId == discordId);
        if (user == null)
        {
            return null;
        }
        
        user.OsuId = osuId;
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<AuthorizedUser?> RemoveAsync(ulong discordId)
    {
        var user = await dbContext.AuthorizedUsers.FirstOrDefaultAsync(u => u.DiscordId == discordId);
        if (user == null)
        {
            return null;
        }
        dbContext.AuthorizedUsers.Remove(user);
        await dbContext.SaveChangesAsync();
        return user;
    }
}