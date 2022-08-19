using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IUnitOfWork unitOfWork, ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Admin>> GetAsync(string userLogin)
    {
        try
        {
            var admin = await _unitOfWork.Admins.GetAsync(w => w.Login == userLogin);

            return admin != null
                ? Result<Admin>.Ok(admin)
                : Result<Admin>.Failed("Пользователь не является администратором");
        }
        catch (Exception ex)
        {
            //#region Log
            //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
            //#endregion
            return Result<Admin>.Failed(ex.Message);
        }
    }
}
