using Portal.Models;

namespace Portal.Services.Interfaces;

public interface ICategoryService
{
    public Task<Result<List<Category>>> GetAllAsync();
    public Task<Result<Category>> GetAsync(int categoryId);
}