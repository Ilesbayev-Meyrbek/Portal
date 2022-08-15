using Portal.CacheManager;
using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;
    private readonly IAdminService _adminService;
    private readonly IRoleService _roleService;
    private readonly ICacheManager _cacheManager;

    public UserService(IUnitOfWork unitOfWork,
        IAdminService adminService,
        IRoleService roleService,
        ICacheManager cacheManager,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _adminService = adminService;
        _roleService = roleService;
        _cacheManager = cacheManager;
        _logger = logger;
    }

    private string GetCacheKey(string userLogin)
    {
        return $"GetUserByLogin_{userLogin}";
    }
    
    public async Task<Result<int>> CreateAsync(User user)
    {
        try
        {
            _unitOfWork.Users.Add(user);
            await _unitOfWork.SaveChangesAsync();

            _cacheManager.Set(user, GetCacheKey(user.Login));
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

            _cacheManager.Set(user, GetCacheKey(user.Login));
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

            _cacheManager.Remove(GetCacheKey(user.Login));
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<bool>.Failed(ex.Message);
        }
    }

    public async Task<Result<User>> GetAsync(string userLogin)
    {
        try
        {
            string keyCache = GetCacheKey(userLogin);
            if (_cacheManager.Get(keyCache) is User userCache)
            {
                return Result<User>.Ok(userCache);
            }
            var user = await _unitOfWork.Users.GetAsync(u => u.Login == userLogin, u => u.Role);

            return user != null ?
                Result<User>.Ok(user) :
                Result<User>.Failed("Пользователь не найден");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<User>.Failed(ex.Message);
        }
    }

    public async Task<Result<List<User>>> GetAllAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllAsync(u => true);
            return Result<List<User>>.Ok(users);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result<List<User>>.Failed(ex.Message);
        }

    }

    public async Task<Result<List<User>>> GetUsersAsync(string userLogin)
    {
        try
        {
            var adminResult = await _adminService.GetAsync(userLogin);

            List<User> usersLst = new List<User>();

            if (adminResult.Success)
            {
                usersLst = await _unitOfWork.Users.GetAllAsync(u => true, u => u.ID, false, include: u => u.Role);
            }
            else
            {
                var user = await _unitOfWork.Users.GetAsync(u => u.Login == userLogin, u => u.Role );

                if (user is {Role: {AdminForScale: true}})
                {
                    var roles = await _roleService.GetAllAsync(r => r.AdminForScale || r.Scales || r.POSs);

                    var roleIds = roles.Data.Select(r => r.ID);

                    usersLst = (await _unitOfWork.Users.GetAllAsync(u => roleIds.Contains(u.RoleID)))
                        .GroupBy(g => g.ID)
                        .Select(s => s.First())
                        .ToList();
                }
            }

            return Result<List<User>>.Ok(usersLst);
        }
        catch (Exception ex)
        {
            //#region Log
            //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
            //#endregion
            return Result<List<User>>.Failed(ex.Message);
        }
    }
    
}
