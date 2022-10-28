using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class ScalesGoodRepository : BaseScaleRepository<ScalesGood>, IScalesGoodRepository
    {
        private readonly ScaleContext _context;

        public ScalesGoodRepository(ScaleContext context) : base(context)
        {
            _context = context;
        }
    }
}