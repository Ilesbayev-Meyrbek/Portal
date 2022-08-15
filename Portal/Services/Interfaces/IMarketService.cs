using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IMarketService
{
    public Task<Result<List<MarketsName>>> GetAllAsync();
}