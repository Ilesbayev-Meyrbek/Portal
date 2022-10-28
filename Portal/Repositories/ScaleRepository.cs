using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class ScaleRepository : BaseScaleRepository<Scale>, IScaleRepository
    {
        private readonly ScaleContext _context;

        public ScaleRepository(ScaleContext context) : base(context)
        {
            _context = context;
        }
    }
}