using Portal.Models;
using Portal.Repositories;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class UserService: IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> CreateAsync(User user)
    {
        try
        {
            _unitOfWork.Users.Add(user);
            await _unitOfWork.SaveChangesAsync();
        
            return Result<int>.Ok(user.ID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<int>.Failed(ex.Message);
        }
        
    }
    
    public async Task<Result<bool>> EditAsync(User user)
    {
        try
        {
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<bool>.Failed(ex.Message);
        }
    }
    
    public async Task<Result<bool>> RemoveAsync(User user)
    {
        try
        {
            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<bool>.Failed(ex.Message);
        }
    }
    
    public async Task<Result<User>> GetAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.ID == userId);

            return Result<User>.Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<User>.Failed(ex.Message);
        }
    }
    
    public async Task<Result<List<User>>> GetAllAsync(User user)
    {
        try
        {
            var users =  await _unitOfWork.Users.GetAllAsync(u => true);
            return Result<List<User>>.Ok(users); 
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result<List<User>>.Failed(ex.Message); 
        }
        
    }
}