using UZ.STS.POS2K.DataAccess;
using Portal.Repositories.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

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