using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class MarketService : IMarketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarketService> _logger;

    public MarketService(IUnitOfWork unitOfWork, ILogger<MarketService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<MarketsName>>> GetAllAsync()
    {
        try
        {
            var markets = await _unitOfWork.Markets.GetAllAsync(w => true);

            return Result<List<MarketsName>>.Ok(markets);
        }
        catch (Exception ex)
        {
            //#region Log
            //CurrentUser _currentUser = (CurrentUser)HttpContext.Current.Session["CurrentUser"];
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", "").Error(ex, ex.Message);
            //#endregion
            return Result<List<MarketsName>>.Failed(ex.Message);
        }
    }
}
