using UZ.STS.POS2K.DataAccess;
using Portal.Repositories.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Repositories
{
    public class UserRepository : BaseRepository<Users>,IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}