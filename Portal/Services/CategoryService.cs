using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminService> _logger;

    public CategoryService(IUnitOfWork unitOfWork, ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<Category>>> GetAllAsync()
    {
        try
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(w => true, m => m.Id, false);

            return Result<List<Category>>.Ok(categories);
        }
        catch (Exception ex)
        {
            return Result <List<Category>>.Failed(ex.Message);
        }
    }
    public async Task<Result<Category>> GetAsync(int categoryId)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetAsync(s => s.Id == categoryId);

            return category != null ?
                Result<Category>.Ok(category) :
                Result<Category>.Failed($"Категория с идентификатором {categoryId} не найдена");
        }
        catch (Exception ex)
        {
            return Result<Category>.Failed(ex.Message);
        }
    }
}