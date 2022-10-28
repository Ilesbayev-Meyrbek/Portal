using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class ScaleService : IScaleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminService> _logger;

    public ScaleService(IUnitOfWork unitOfWork, ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<Scale>>> GetAllAsync()
    {
        try
        {
            var scales = await _unitOfWork.Scales.GetAllAsync(w => true, m => m.ID, false);

            return Result<List<Scale>>.Ok(scales);
        }
        catch (Exception ex)
        {
            return Result <List<Scale>>.Failed(ex.Message);
        }
    }
    public async Task<Result<Scale>> GetAsync(long scaleId)
    {
        try
        {
            var scale = await _unitOfWork.Scales.GetAsync(s => s.ID == scaleId);

            return scale != null ?
                Result<Scale>.Ok(scale) :
                Result<Scale>.Failed($"Весы с идентификатором {scaleId} не найдены");
        }
        catch (Exception ex)
        {
            return Result<Scale>.Failed(ex.Message);
        }
    }
    public async Task<Result<Scale>> GetByIPAsync(string scaleIP)
    {
        try
        {
            var scale = await _unitOfWork.Scales.GetAsync(s => s.IP == scaleIP);

            return scale != null ?
                Result<Scale>.Ok(scale) :
                Result<Scale>.Failed($"Весы с IP {scaleIP} не найдены");
        }
        catch (Exception ex)
        {
            return Result<Scale>.Failed(ex.Message);
        }
    }
    public async Task<Result> ChangeCategory(Scale scale, int categoryIndex)
    {
        try
        {
            scale.DefaultCategoryIndex = categoryIndex;
            scale.DataUpdateTime = DateTime.Now;
            await _unitOfWork.SaveScaleChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Failed(ex.Message);
        }
    }
}