using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OsuCzechiaBot.Models.Interfaces;

namespace OsuCzechiaBot.Database.DatabaseServices;

public abstract class DatabaseServiceBase<T, TId>(OsuCzechiaBotDatabaseContext dbContext, DbSet<T> dbSet) where T : class, IIdentifiable<TId> where TId : IEquatable<TId>
{
    protected readonly DbSet<T> DbSet = dbSet;
    protected readonly OsuCzechiaBotDatabaseContext Context = dbContext;

    public virtual async Task<bool> ExistsAsync(TId id)
    {
        return await DbSet.AnyAsync(e => e.Id.Equals(id));
    }
    
    public async Task<T> AddAsync(T entity)
    {
        DbSet.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> AddOrUpdateAsync(T entity)
    {
        EntityEntry<T> result;
        if (!await DbSet.AnyAsync(e => e.Id.Equals(entity.Id)))
        {
            result = DbSet.Add(entity);
        }
        else
        {
            result = DbSet.Update(entity);
        }

        await Context.SaveChangesAsync();
        return result.Entity;
    }

    public virtual async Task<T?> UpdateAsync(T entity)
    {
        if (!await DbSet.AnyAsync(e => e.Id.Equals(entity.Id)))
        {
            return null;
        }
        
        var result = DbSet.Update(entity);

        await Context.SaveChangesAsync();
        return result.Entity;
    }

    public virtual async Task<T?> GetAsync(TId id)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id.Equals(id));
    }

    public virtual async Task<T?> GetByAsync(Expression<Func<T,bool>> predicate)
    {
        return await DbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IReadOnlyList<T>> GetManyAsync(List<TId> idList)
    {
        return await DbSet.Where(e => idList.Contains(e.Id)).ToListAsync();
    }

    public virtual async Task<T?> RemoveAsync(TId id)
    {
        var entity = await DbSet.FirstOrDefaultAsync(e => e.Id.Equals(id));
        if (entity == null)
        {
            return null;
        }

        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
        return entity;
    }
}