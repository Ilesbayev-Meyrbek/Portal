
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Services.Interfaces;

public interface IMarketService
{
    public Task<Result<MarketsName>> GetAsync(string marketId);
    public Task<Result<List<MarketsName>>> GetAllAsync();
}