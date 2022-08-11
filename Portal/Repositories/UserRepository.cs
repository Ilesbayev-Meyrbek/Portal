using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class UserRepository : BaseRepository<User>,IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}