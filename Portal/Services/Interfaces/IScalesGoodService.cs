using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IScalesGoodService
{
    public Task<Result<ScalesGood>> GetAsync(long goodId);
    public Task<Result<List<ScalesGood>>> GetGoodsAsync(string marketId);
    public Task<Result<List<ScalesGood>>> GetGoodsWithoutImgAsync(string marketId);
    public Task<Result> SetGoodImage(IFormFile file, string goodIds);
}