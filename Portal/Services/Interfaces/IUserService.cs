using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IUserService
{

    public Task<Result<int>> CreateAsync(User user);

    public Task<Result<bool>> EditAsync(User user);

    public Task<Result<bool>> RemoveAsync(User user);

    public Task<Result<User>> GetAsync(int userId);

    public Task<Result<List<User>>> GetAllAsync(User user);
}