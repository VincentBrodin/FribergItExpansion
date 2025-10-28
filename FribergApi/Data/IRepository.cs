using System.Linq.Expressions;

namespace FribergApi.Data;


public interface IRepository<T> where T : class
{
    void Update(T entity);
    void Remove(T entity);

    Task AddAsync(T entity);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> AllAsync();
    Task<int> SaveChangesAsync();
}
