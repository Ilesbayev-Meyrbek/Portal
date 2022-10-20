using UZ.STS.POS2K.DataAccess;
using Portal.Repositories.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Repositories
{
    public class RoleRepository : BaseRepository<Roles>,IRoleRepository
    {
        private readonly DataContext _context;

        public RoleRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}