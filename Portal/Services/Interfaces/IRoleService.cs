using System.Linq.Expressions;
using Portal.Models;

namespace Portal.Services.Interfaces;

public interface IRoleService
{
    public Task<Result<Role>> GetAsync(string userLogin);
    public Task<Result<List<Role>>> GetAllAsync(Expression<Func<Role, bool>> predicate);
}