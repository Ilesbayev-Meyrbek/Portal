using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IMarketCategoryService
{
    public Task<Result<List<MarketCategory>>> GetForMarketAsync(string marketId);
    public Task<Result> ChangeCategoriesOrderAsync(List<MarketCategory> categoriesOrder, string categoryIdsOrder);
}