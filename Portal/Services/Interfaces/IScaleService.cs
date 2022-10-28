using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IScaleService
{
    public Task<Result<List<Scale>>> GetAllAsync();
    public Task<Result<Scale>> GetAsync(long scaleId);
    public Task<Result<Scale>> GetByIPAsync(string scaleIP);
    public Task<Result> ChangeCategory(Scale scale, int categoryIndex);
}