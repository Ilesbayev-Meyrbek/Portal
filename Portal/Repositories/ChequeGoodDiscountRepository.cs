using Portal.Repositories.Interfaces;
using UZ.STS.POS2K.DataAccess;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Repositories
{
    public class ChequeGoodDiscountRepository : BaseRepository<ChequeGoodDiscount>, IChequeGoodDiscountRepository
    {
        private readonly DataContext _context;

        public ChequeGoodDiscountRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
