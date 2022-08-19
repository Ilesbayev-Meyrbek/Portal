using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class MarketRepository : BaseRepository<MarketsName>,IMarketRepository
    {
        private readonly DataContext _context;

        public MarketRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}