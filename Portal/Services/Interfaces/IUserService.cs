using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IUserService
{

    public Task<Result<int>> CreateAsync(User user);

    public Task<Result<bool>> EditAsync(User user);

    public Task<Result<bool>> RemoveAsync(User user);

    public Task<Result<User>> GetAsync(string userLogin);

    public Task<Result<List<User>>> GetAllAsync();

    public Task<Result<List<User>>> GetUsersAsync(string userLogin);
}