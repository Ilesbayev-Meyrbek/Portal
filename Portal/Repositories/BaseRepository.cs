using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Portal.DB;
using Portal.Repositories.Interfaces;
using Portal.Extensions;
using QueryableExtensions = System.Data.Entity.QueryableExtensions;

namespace Portal.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T: class
    {
        private readonly DataContext _db;

        protected BaseRepository(DataContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate">Filter</param>
        /// <param name="include">Example: p => p.AccountChart, p => p.Currency</param>
        /// <returns></returns>
        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] include
            )
        {
            IQueryable<T> query = _db.Set<T>();
            if (include.Any())
                query = include.Aggregate(query, (current, inc) => current.Include(inc));

            return await query.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate">Filter</param>
        /// <param name="orderByProperty">Default value: Created</param>
        /// <param name="orderByDescending">Default value:true</param>
        /// <param name="page">Page number</param>
        /// <param name="count">Count elements</param>
        /// <param name="include">Example: p => p.AccountChart, p => p.Currency</param>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>>? orderByProperty = null, 
            bool orderByDescending = true,
            int page = 1,
            int count = 1000,
            params Expression<Func<T, object>>[] include
            )
        {
            IQueryable<T> query = _db.Set<T>();
            
            if (include.Any()) 
                query = include.Aggregate(query, (current, inc) => current.Include(inc));

            return await QueryableExtensions.ToListAsync(query.Where(predicate).OrderBy(orderByProperty, orderByDescending).Skip(((page <= 0 ? 1 : page) - 1) * count).Take(count));
        }

        public virtual void Add(T entity) => _db.Set<T>().Add(entity);
        public virtual void Update(T entity) => _db.Set<T>().Update(entity);

        public virtual void Remove(T entity) => _db.Set<T>().Remove(entity);

        public virtual Task<int> CountAsync(Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
            => _db.Set<T>().CountAsync(predicate, cancellationToken);
        public virtual Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default
            )
            => _db.Set<T>().AnyAsync(predicate, cancellationToken);
    }
}