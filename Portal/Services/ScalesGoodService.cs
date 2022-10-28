using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class ScalesGoodService : IScalesGoodService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminService> _logger;

    public ScalesGoodService(IUnitOfWork unitOfWork, ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result<ScalesGood>> GetAsync(long goodId)
    {
        try
        {
            var good = await _unitOfWork.ScalesGoods.GetAsync(g => g.ID == goodId);

            return good != null ?
                Result<ScalesGood>.Ok(good) :
                Result<ScalesGood>.Failed($"Товар с идентификатором {goodId} не найден");
        }
        catch (Exception ex)
        {
            return Result<ScalesGood>.Failed(ex.Message);
        }
    }
    public async Task<Result<List<ScalesGood>>> GetGoodsAsync(string marketId)
    {
        try
        {
            var goods = await _unitOfWork.ScalesGoods.GetAllAsync(p => p.MarketID == marketId && p.GroupId != null, g => g.ID, false, 1, 1000, x => x.Group);

            return Result<List<ScalesGood>>.Ok(goods);
        }
        catch (Exception ex)
        {
            return Result<List<ScalesGood>>.Failed(ex.Message);
        }
    }

    public async Task<Result<List<ScalesGood>>> GetGoodsWithoutImgAsync(string marketId)
    {
        try
        {
            var goods = await _unitOfWork.ScalesGoods.GetAllAsync(p => p.MarketID == marketId && p.GroupId != null && p.ImageId == null, g => g.ID, false, 1, 1000, x => x.Group);

            return Result<List<ScalesGood>>.Ok(goods);
        }
        catch (Exception ex)
        {
            return Result<List<ScalesGood>>.Failed(ex.Message);
        }
    }
    public async Task<Result> SetGoodImage(IFormFile file, string goodIds)
    {
        try
        {
            var listGoodIds = goodIds.Split(',').Select(x => long.Parse(x)).ToList();
            var goods = await _unitOfWork.ScalesGoods.GetAllAsync(x => listGoodIds.Contains(x.ID), g => g.ID);

            string bDate;
            string bThumb;
            System.Drawing.Image thumbnail;
            System.Drawing.Image sourceimage;

            sourceimage = System.Drawing.Image.FromStream(file.OpenReadStream());

            Image temp = new Image();
            using (var ms = new MemoryStream())
            {
                sourceimage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bDate = Convert.ToBase64String(ms.ToArray());
            }

            temp.Data = bDate;
            thumbnail = sourceimage.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

            using (var ms = new MemoryStream())
            {
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bThumb = Convert.ToBase64String(ms.ToArray());
            }
            temp.Thumbnail = bThumb;
            temp.UpdateTime = DateTime.Now;
            _unitOfWork.Images.Add(temp);
            await _unitOfWork.SaveScaleChangesAsync();

            foreach (var good in goods)
            {
                var goodInDB = (await GetAsync(good.ID)).Data;
                goodInDB.Image = temp;
                goodInDB.ImageId = temp.Id;
                goodInDB.ReceiptGoodDate = DateTime.Now;
            }
            await _unitOfWork.SaveScaleChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Failed(ex.Message);
        }
    }
}