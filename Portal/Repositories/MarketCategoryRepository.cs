using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class MarketCategoryRepository : BaseRepository<MarketCategory>, IMarketCategoryRepository
    {
        private readonly DataContext _context;

        public MarketCategoryRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}