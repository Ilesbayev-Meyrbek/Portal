using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IGroupService
{
    public Task<Result<List<Group>>> GetAllAsync();
    public Task<Result<Group>> GetAsync(string groupPlu);
}