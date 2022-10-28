using Portal.Repositories.Interfaces;

namespace Portal.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveScaleChangesAsync();
        
        IUserRepository Users { get; }
        IAdminRepository Admins { get; }
        IRoleRepository Roles { get; }
        IMarketRepository Markets { get; }
        ICategoryRepository Categories { get; }
        IMarketCategoryRepository MarketCategories { get; }
        IScaleRepository Scales { get; }
        IGroupRepository Groups { get; }
        IScalesGoodRepository ScalesGoods { get; }
        IImageRepository Images { get; }
    }
}