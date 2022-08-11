using System.Linq.Expressions;

namespace Portal.Repositories.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<T> GetAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] include
            );

        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>>? orderByProperty = null,
            bool orderByDescending = true, int page = 1, int count = 1000,
            params Expression<Func<T, object>>[] include
            );

        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}