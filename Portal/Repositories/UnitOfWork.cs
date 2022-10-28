using Portal.DB;
using Portal.Repositories.Interfaces;

namespace Portal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _db;
        private readonly ScaleContext _scaleContext;
        private readonly Lazy<IUserRepository> _user;
        private readonly Lazy<IAdminRepository> _admin;
        private readonly Lazy<IRoleRepository> _role;
        private readonly Lazy<IMarketRepository> _market;
        private readonly Lazy<ICategoryRepository> _category;
        private readonly Lazy<IMarketCategoryRepository> _marketCategory;
        private readonly Lazy<IScaleRepository> _scale;
        private readonly Lazy<IGroupRepository> _group;
        private readonly Lazy<IScalesGoodRepository> _scalesGood;
        private readonly Lazy<IImageRepository> _image;

        public UnitOfWork(DataContext db, ScaleContext scaleContext)
        {
            _db = db;
            _scaleContext = scaleContext;
            _user = new Lazy<IUserRepository>(() => new UserRepository(db));
            _admin = new Lazy<IAdminRepository>(() => new AdminRepository(db));
            _role = new Lazy<IRoleRepository>(() => new RoleRepository(db));
            _market = new Lazy<IMarketRepository>(() => new MarketRepository(db));
            _category = new Lazy<ICategoryRepository>(() => new CategoryRepository(db));
            _marketCategory = new Lazy<IMarketCategoryRepository>(() => new MarketCategoryRepository(db));
            _scale = new Lazy<IScaleRepository>(() => new ScaleRepository(scaleContext));
            _group = new Lazy<IGroupRepository>(() => new GroupRepository(scaleContext));
            _scalesGood = new Lazy<IScalesGoodRepository>(() => new ScalesGoodRepository(scaleContext));
            _image = new Lazy<IImageRepository>(() => new ImageRepository(scaleContext));
        }

        public Task<int> SaveChangesAsync() =>
            _db.SaveChangesAsync();
        public Task<int> SaveScaleChangesAsync() =>
            _scaleContext.SaveChangesAsync();

        public IUserRepository Users => _user.Value;
        
        public IAdminRepository Admins => _admin.Value;
        public IRoleRepository Roles => _role.Value;
        public IMarketRepository Markets => _market.Value;
        public ICategoryRepository Categories => _category.Value;
        public IMarketCategoryRepository MarketCategories => _marketCategory.Value;
        public IScaleRepository Scales => _scale.Value;
        public IGroupRepository Groups => _group.Value;
        public IScalesGoodRepository ScalesGoods => _scalesGood.Value;
        public IImageRepository Images => _image.Value;
    }
}