using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class MarketCategoryService : IMarketCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminService> _logger;

    public MarketCategoryService(IUnitOfWork unitOfWork, ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<MarketCategory>>> GetForMarketAsync(string marketId)
    {
        try
        {
            var categories = await _unitOfWork.MarketCategories.GetAllAsync(w => w.MarketId == marketId, m => m.MarketId, false, include: i => i.Category);

            return Result<List<MarketCategory>>.Ok(categories);
        }
        catch (Exception ex)
        {
            return Result <List<MarketCategory>>.Failed(ex.Message);
        }
    }
    public async Task<Result> ChangeCategoriesOrderAsync(List<MarketCategory> categoriesOrder, string categoryIdsOrder)
    {
        try
        {
            var listString = categoryIdsOrder.Split(',');
            var listInt = new List<int>();

            for (int i = 0; i < listString.Length; i++)
                listInt.Add(int.Parse(listString[i]));

            for (int i = 0; i < listInt.Count(); i++)
                foreach (var category in categoriesOrder)
                    if (category.CategoryId == listInt[i])
                    {
                        category.CategoryOrder = i;
                        category.UpdateTime = DateTime.Now;
                    }

            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Failed(ex.Message);
        }
    }
}