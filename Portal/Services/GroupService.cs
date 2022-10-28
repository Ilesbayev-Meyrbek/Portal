using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services;

public class GroupService : IGroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminService> _logger;

    public GroupService(IUnitOfWork unitOfWork, ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<Group>>> GetAllAsync()
    {
        try
        {
            var groups = await _unitOfWork.Groups.GetAllAsync(w => true, g => g.Id, false);

            return Result<List<Group>>.Ok(groups);
        }
        catch (Exception ex)
        {
            return Result <List<Group>>.Failed(ex.Message);
        }
    }
    public async Task<Result<Group>> GetAsync(string groupPlu)
    {
        try
        {
            var group = await _unitOfWork.Groups.GetAsync(g => g.SapID == groupPlu);

            return group != null ?
                Result<Group>.Ok(group) :
                Result<Group>.Failed($"Группа с идентификатором {groupPlu} не найдена");
        }
        catch (Exception ex)
        {
            return Result<Group>.Failed(ex.Message);
        }
    }
}