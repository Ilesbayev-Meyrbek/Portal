using Microsoft.EntityFrameworkCore;
using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net.WebSockets;

namespace Portal.Repositories
{
    public class ChequeBottomMessageRepository : BaseRepository<ChequeBottomMessageRepository>, IChequeBottomMessageRepository
    {
        private readonly DataContext _context;

        public ChequeBottomMessageRepository(DataContext context) : base(context)
        {
            _context=context;
        }

        public void Add(ChequeBottomMessage entity) 
            => _context.Add(entity);

        public async Task<bool> AnyAsync(Expression<Func<ChequeBottomMessage, bool>> predicate, CancellationToken cancellationToken = default)
            => await _context.ChequeBottomMessages.AnyAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(Expression<Func<ChequeBottomMessage, bool>> predicate, CancellationToken cancellationToken = default)
            => await _context.ChequeBottomMessages.CountAsync(predicate, cancellationToken);

        public async Task<List<ChequeBottomMessage>> GetAllAsync(Expression<Func<ChequeBottomMessage, bool>> predicate, Expression<Func<ChequeBottomMessage, object>>? orderByProperty = null, bool orderByDescending = true, int page = 1, int count = 1000, params Expression<Func<ChequeBottomMessage, object>>[] include)
        {
            var r= _context.ChequeBottomMessages.Where(predicate);
            if (orderByProperty is not null)
                r =orderByDescending ? r.OrderByDescending(orderByProperty):  r.OrderBy(orderByProperty);
            foreach (var item in include)
            {
                r = r.Include(item);
            }
            return await r.ToListAsync();
        }

        public async Task<ChequeBottomMessage?> GetAsync(Expression<Func<ChequeBottomMessage, bool>> predicate, params Expression<Func<ChequeBottomMessage, object>>[] include)
        {
            IQueryable<ChequeBottomMessage> r=_context.ChequeBottomMessages;
            foreach (var item in include)
            {
                r=r.Include(item);
            }
            return await r.FirstOrDefaultAsync(predicate);
        }

        public void Remove(ChequeBottomMessage entity)
            => _context.Remove(entity);

        public void Update(ChequeBottomMessage entity) 
            => _context.Update(entity);
    }
}
