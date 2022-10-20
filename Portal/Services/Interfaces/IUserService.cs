using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Services.Interfaces;

public interface IUserService
{
    public Task<Users> GetCurrentUser();
    public Task<Result<int>> CreateAsync(Users user);

    public Task<Result> EditAsync(Users user);

    public Task<Result> RemoveAsync(int userId);

    public Task<Result<Users>> GetAsync(string userLogin);
    public Task<Result<Users>> GetAsync(int userId);

    public Task<Result<List<Users>>> GetAllAsync();

    public Task<Result<List<Users>>> GetUsersAsync(string userLogin);
}