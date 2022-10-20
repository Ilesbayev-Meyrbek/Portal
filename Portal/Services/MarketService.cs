using Portal.Services.Interfaces;
using Portal.Repositories.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

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

    public async Task<Result<MarketsName>> GetAsync(string marketId)
    {
        try
        {
            var market = await _unitOfWork.Markets.GetAsync(m => m.MarketID == marketId, include: p => p.Poses);

            return market != null ?
                Result<MarketsName>.Ok(market) :
                Result<MarketsName>.Failed($"Маркет с идентификатором {marketId} не найден");
        }
        catch (Exception ex)
        {
            return Result<MarketsName>.Failed(ex.Message);
        }
    }
    public async Task<Result<List<MarketsName>>> GetAllAsync()
    {
        try
        {
            var markets = await _unitOfWork.Markets.GetAllAsync(w => true, m => m.Name, orderByDescending: false, include: p => p.Poses);

            return Result<List<MarketsName>>.Ok(markets);
        }
        catch (Exception ex)
        {
            return Result<List<MarketsName>>.Failed(ex.Message);
        }
    }
}
