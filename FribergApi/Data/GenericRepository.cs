
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace FribergApi.Data;

public class GenericRepository<T>(ApiContext context) : IRepository<T> where T : class
{
    public virtual async Task AddAsync(T entity)
    {
        await context.AddAsync(entity);
    }

    public virtual async Task<IEnumerable<T>> AllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    public virtual void Remove(T entity)
    {
        context.Remove(entity);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>().Where(predicate).ToListAsync();
    }

    public virtual Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public virtual Task<int> SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }

    public virtual void Update(T entity)
    {
        context.Update(entity);
    }
}
