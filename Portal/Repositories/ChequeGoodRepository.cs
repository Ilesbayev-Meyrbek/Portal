using Portal.DB;
using Portal.Extensions;
using Portal.Repositories.Interfaces;
using System.Data.Entity;
using System.Linq.Expressions;
using UZ.STS.POS2K.DataAccess;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Repositories
{
    public class ChequeGoodRepository : BaseRepository<ChequeGood>, IChequeGoodRepository
    {
        private readonly DataContext _context;

        public ChequeGoodRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
