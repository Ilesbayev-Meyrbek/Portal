using UZ.STS.POS2K.DataAccess;
using Portal.Repositories.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Repositories
{
    public class ChequeRepository : BaseRepository<Cheque>, IChequeRepository
    {
        private readonly DataContext _context;

        public ChequeRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
