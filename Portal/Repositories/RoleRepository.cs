using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class RoleRepository : BaseRepository<Role>,IRoleRepository
    {
        private readonly DataContext _context;

        public RoleRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}