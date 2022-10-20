using Portal.Repositories.Interfaces;
using UZ.STS.POS2K.DataAccess;

namespace Portal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _db;
        private readonly Lazy<IUserRepository> _user;
        private readonly Lazy<IRoleRepository> _role;
        private readonly Lazy<IMarketRepository> _market;
        private readonly Lazy<IChequeRepository> _cheque;
        private readonly Lazy<IChequeGoodRepository> _chequeGood;
        private readonly Lazy<IChequeGoodDiscountRepository> _chequeGoodDiscount;

        public UnitOfWork(DataContext db)
        {
            _db = db;
            _user = new Lazy<IUserRepository>(() => new UserRepository(db));
            _role = new Lazy<IRoleRepository>(() => new RoleRepository(db));
            _market = new Lazy<IMarketRepository>(() => new MarketRepository(db));
            _cheque = new Lazy<IChequeRepository>(() => new ChequeRepository(db));
            _chequeGood = new Lazy<IChequeGoodRepository>(() => new ChequeGoodRepository(db));
            _chequeGoodDiscount = new Lazy<IChequeGoodDiscountRepository>(() => new ChequeGoodDiscountRepository(db));
        }

        public Task<int> SaveChangesAsync() =>
            _db.SaveChangesAsync();
        
        public IUserRepository Users => _user.Value;
        
        public IRoleRepository Roles => _role.Value;
        public IMarketRepository Markets => _market.Value;

        public IChequeRepository Cheques => _cheque.Value;
        public IChequeGoodRepository ChequeGoods => _chequeGood.Value;
        public IChequeGoodDiscountRepository ChequeGoodDiscounts => _chequeGoodDiscount.Value;
    }
}