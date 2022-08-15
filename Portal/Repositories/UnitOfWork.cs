using Portal.DB;
using Portal.Repositories.Interfaces;

namespace Portal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _db;
        private readonly Lazy<IUserRepository> _user;
        private readonly Lazy<IAdminRepository> _admin;
        private readonly Lazy<IRoleRepository> _role;
        private readonly Lazy<IMarketRepository> _market;

        public UnitOfWork(DataContext db)
        {
            _db = db;
            _user = new Lazy<IUserRepository>(() => new UserRepository(db));
            _admin = new Lazy<IAdminRepository>(() => new AdminRepository(db));
            _role = new Lazy<IRoleRepository>(() => new RoleRepository(db));
            _market = new Lazy<IMarketRepository>(() => new MarketRepository(db));
        }

        public Task<int> SaveChangesAsync() =>
            _db.SaveChangesAsync();
        
        public IUserRepository Users => _user.Value;
        
        public IAdminRepository Admins => _admin.Value;
        public IRoleRepository Roles => _role.Value;
        public IMarketRepository Markets => _market.Value;
    }
}