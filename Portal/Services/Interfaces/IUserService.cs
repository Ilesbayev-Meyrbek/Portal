using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IUserService
{
    public Task<User> GetCurrentUser();
    public Task<Result<int>> CreateAsync(User user);

    public Task<Result> EditAsync(User user);

    public Task<Result> RemoveAsync(int userId); 

    public Task<Result<User>> GetAsync(string userLogin);
    public Task<Result<User>> GetAsync(int userId);

    public Task<Result<List<User>>> GetAllAsync();

    public Task<Result<List<User>>> GetUsersAsync(string userLogin);
}