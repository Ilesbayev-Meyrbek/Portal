using Portal.DB;
using Portal.Repositories.Interfaces;

namespace Portal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _db;
        private readonly Lazy<IUserRepository> _user;

        public UnitOfWork(DataContext db)
        {
            _db = db;
            _user = new Lazy<IUserRepository>(() => new UserRepository(db));
        }

        public Task<int> SaveChangesAsync() =>
            _db.SaveChangesAsync();
        
        public IUserRepository Users => _user.Value;
    }
}