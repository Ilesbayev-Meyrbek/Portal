using Portal.Repositories.Interfaces;

namespace Portal.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        
        IUserRepository Users { get; }
        IAdminRepository Admins { get; }
        IRoleRepository Roles { get; }
        IMarketRepository Markets { get; }
    }
}