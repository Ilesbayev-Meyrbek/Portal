using System.Linq.Expressions;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoleService> _logger;

    public RoleService(IUnitOfWork unitOfWork, ILogger<RoleService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<Roles>>> GetAllAsync(Expression<Func<Roles, bool>> predicate)
    {
        try
        {
            var roles = await _unitOfWork.Roles.GetAllAsync(predicate, r => r.Name, false);

            return Result<List<Roles>>.Ok(roles);
        }
        catch (Exception ex)
        {
            //#region Log
            //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
            //#endregion
            return Result<List<Roles>>.Failed(ex.Message);
        }
    }

    Task<Result<Roles>> IRoleService.GetAsync(string userLogin)
    {
        throw new NotImplementedException();
    }
}
