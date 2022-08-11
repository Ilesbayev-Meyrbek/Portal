using Portal.Repositories.Interfaces;

namespace Portal.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        
        IUserRepository Users { get; }
    }
}