using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IAdminService
{
    public Task<Result<Admin>> GetAsync(string userLogin);
}