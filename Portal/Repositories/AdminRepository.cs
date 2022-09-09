using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        private readonly DataContext _context;

        public AdminRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}