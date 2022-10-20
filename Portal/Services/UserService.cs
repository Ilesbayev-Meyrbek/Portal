using Portal.CacheManager;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;
    private readonly IRoleService _roleService;
    private readonly ICacheManager _cacheManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IUnitOfWork unitOfWork,
        IRoleService roleService,
        ICacheManager cacheManager,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _roleService = roleService;
        _cacheManager = cacheManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Users> GetCurrentUser()
    {
        string login = _httpContextAccessor.HttpContext.User.Identity.Name;

        var userResult = await GetAsync(login);

        if (userResult.Success)
            return userResult.Data;

        throw new Exception(userResult.Message);
    }

    public async Task<Result<Users>> GetAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.ID == userId, u => u.Roles, u => u.Market);

            return user != null ?
                Result<Users>.Ok(user) :
                Result<Users>.Failed("Пользователь не найден");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<Users>.Failed(ex.Message);
        }
    }

    public async Task<Result<Users>> GetAsync(string userLogin)
    {
        try
        {
            string keyCache = GetCacheKey(userLogin);
            if (_cacheManager.Get(keyCache) is Users userCache)
            {
                return Result<Users>.Ok(userCache);
            }

            var user = await _unitOfWork.Users.GetAsync(u => u.Login == userLogin, u => u.Roles, u => u.Market);
            
            if(user == null)
                return Result<Users>.Failed($"Пользователь с логином {userLogin} не найден.");
            
            _cacheManager.Set(user, keyCache);

            return Result<Users>.Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<Users>.Failed(ex.Message);
        }
    }

    public async Task<Result<List<Users>>> GetAllAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllAsync(u => true);
            return Result<List<Users>>.Ok(users);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result<List<Users>>.Failed(ex.Message);
        }

    }

    public async Task<Result<List<Users>>> GetUsersAsync(string userLogin)
    {
        try
        {
            var userResult = await GetAsync(userLogin);

            List<Users> usersLst = new List<Users>();

            if (userResult.Success && userResult.Data.IsAdmin)
            {
                usersLst = await _unitOfWork.Users.GetAllAsync(u => true, u => u.ID, true);
            }
            else if (userResult.Success && !userResult.Data.IsAdmin)
            {
                var user = await _unitOfWork.Users.GetAsync(u => u.Login == userLogin, u => u.Roles);

                if (user is { Roles: { AdminForScale: true } })
                {
                    var roles1 = await _roleService.GetAllAsync(r => r.AdminForScale && r.Name != "Admin");
                    var roles2 = await _roleService.GetAllAsync(r => (r.Scales || r.POSs) && !r.CreateCashiers && !r.EditCashiers && !r.DeleteCashiers && !r.CreateLogo && !r.EditLogo && !r.DeleteLogo && !r.CreateKeyboard && !r.EditKeyboard && !r.DeleteKeyboard);
                    var roles = roles1.Data.Concat(roles2.Data);

                    var roleIds = roles.Select(r => r.ID);

                    usersLst = (await _unitOfWork.Users.GetAllAsync(u => roleIds.Contains(u.RoleID), u => u.ID))
                        .Where(w => !w.IsAdmin)
                        .GroupBy(g => g.ID)
                        .Select(s => s.First())
                        .ToList();
                }
            }

            return Result<List<Users>>.Ok(usersLst);
        }
        catch (Exception ex)
        {
            //#region Log
            //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
            //#endregion
            return Result<List<Users>>.Failed(ex.Message);
        }
    }

    public async Task<Result<int>> CreateAsync(Users user)
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

    public async Task<Result> EditAsync(Users user)
    {
        try
        {
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _cacheManager.Set(user, GetCacheKey(user.Login));
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failed(ex.Message);
        }
    }

    public async Task<Result> RemoveAsync(int userId)
    {
        try
        {
            var userResult = await GetAsync(userId);
            if (userResult.Success)
            {
                _unitOfWork.Users.Remove(userResult.Data);
                await _unitOfWork.SaveChangesAsync();

                _cacheManager.Remove(GetCacheKey(userResult.Data.Login));
                return Result.Ok();
            }
            _logger.LogWarning("Пользователь не найден");
            return Result.Failed("Пользователь не найден");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failed(ex.Message);
        }
    }

    private string GetCacheKey(string userLogin)
    {
        return $"GetUserByLogin_{userLogin}";
    }
}
