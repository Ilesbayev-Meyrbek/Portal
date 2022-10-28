using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class ImageService : IImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminService> _logger;

    public ImageService(IUnitOfWork unitOfWork, ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Image>> GetAsync(int imageId)
    {
        try
        {
            var image = await _unitOfWork.Images.GetAsync(g => g.Id == imageId);

            return image != null ?
                Result<Image>.Ok(image) :
                Result<Image>.Failed($"Изображение с идентификатором {imageId} не найдено");
        }
        catch (Exception ex)
        {
            return Result<Image>.Failed(ex.Message);
        }
    }
}