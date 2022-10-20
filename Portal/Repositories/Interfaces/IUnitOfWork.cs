
namespace Portal.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IMarketRepository Markets { get; }
        IChequeRepository Cheques { get; }
        IChequeGoodRepository ChequeGoods { get; }
        IChequeGoodDiscountRepository ChequeGoodDiscounts { get; }
    }
}