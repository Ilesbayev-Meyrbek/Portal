using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IMarketService
{
    public Task<Result<MarketsName>> GetAsync(string marketId);
    public Task<Result<List<MarketsName>>> GetAllAsync();
}