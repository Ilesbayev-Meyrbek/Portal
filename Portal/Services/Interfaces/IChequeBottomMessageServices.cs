using Portal.Models;

namespace Portal.Services.Interfaces
{
    public interface IChequeBottomMessageServices
    {
        public Task<Result<int>> CreateAsync(ChequeBottomMessage user);

        public Task<Result> EditAsync(ChequeBottomMessage user);

        public Task<Result> RemoveAsync(int userId);

        public Task<Result<ChequeBottomMessage>> GetAsync(int userId);

        public Task<Result<List<ChequeBottomMessage>>> GetAllAsync();
    }
}
