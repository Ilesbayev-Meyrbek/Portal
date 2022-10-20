using System.Linq.Expressions;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Services.Interfaces;

public interface IRoleService
{
    public Task<Result<Roles>> GetAsync(string userLogin);
    public Task<Result<List<Roles>>> GetAllAsync(Expression<Func<Roles, bool>> predicate);
}