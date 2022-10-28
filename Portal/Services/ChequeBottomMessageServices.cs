using Portal.CacheManager;
using Portal.Models;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;

namespace Portal.Services
{
    public class ChequeBottomMessageServices : IChequeBottomMessageServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChequeBottomMessageServices> _logger;
        private readonly IAdminService _adminService;
        private readonly IRoleService _roleService;
        private readonly ICacheManager _cacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChequeBottomMessageServices(IUnitOfWork unitOfWork,
            IAdminService adminService,
            IRoleService roleService,
            ICacheManager cacheManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChequeBottomMessageServices> logger)
        {
            _unitOfWork = unitOfWork;
            _adminService = adminService;
            _roleService = roleService;
            _cacheManager = cacheManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }



        public async Task<Result<ChequeBottomMessage>> GetAsync(int Id)
        {
            try
            {
                var ChequeBottomMessage = await _unitOfWork.ChequeBottomMessage.GetAsync(u => u.ID == Id);

                return ChequeBottomMessage != null ?
                    Result<ChequeBottomMessage>.Ok(ChequeBottomMessage) :
                    Result<ChequeBottomMessage>.Failed("сообщение не найдено");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result<ChequeBottomMessage>.Failed(ex.Message);
            }
        }


        public async Task<Result<List<ChequeBottomMessage>>> GetAllAsync()
        {
            try
            {
                var ChequeBottomMessages = await _unitOfWork.ChequeBottomMessage.GetAllAsync(u => true);
                return Result<List<ChequeBottomMessage>>.Ok(ChequeBottomMessages);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Result<List<ChequeBottomMessage>>.Failed(ex.Message);
            }

        }

        public async Task<Result<int>> CreateAsync(ChequeBottomMessage ChequeBottomMessage)
        {
            try
            {
                _unitOfWork.ChequeBottomMessage.Add(ChequeBottomMessage);
                await _unitOfWork.SaveChangesAsync();

                _cacheManager.Set(ChequeBottomMessage, GetCacheKey(ChequeBottomMessage.ID.ToString()));
                return Result<int>.Ok(ChequeBottomMessage.ID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result<int>.Failed(ex.Message);
            }

        }

        public async Task<Result> EditAsync(ChequeBottomMessage ChequeBottomMessage)
        {
            try
            {
                _unitOfWork.ChequeBottomMessage.Update(ChequeBottomMessage);
                await _unitOfWork.SaveChangesAsync();

                _cacheManager.Set(ChequeBottomMessage, GetCacheKey(ChequeBottomMessage.ID.ToString()));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failed(ex.Message);
            }
        }

        public async Task<Result> RemoveAsync(int ChequeBottomMessageId)
        {
            try
            {
                var ChequeBottomMessageResult = await GetAsync(ChequeBottomMessageId);
                if (ChequeBottomMessageResult.Success)
                {
                    _unitOfWork.ChequeBottomMessage.Remove(ChequeBottomMessageResult.Data);
                    await _unitOfWork.SaveChangesAsync();

                    _cacheManager.Remove(GetCacheKey(ChequeBottomMessageResult.Data.ID.ToString()));
                    return Result.Ok();
                }
                _logger.LogWarning("сообщение не найдено");
                return Result.Failed("сообщение не найдено");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Failed(ex.Message);
            }
        }

        private string GetCacheKey(string ChequeBottomMessageLogin)
        {
            return $"GetChequeBottomMessageByLogin_{ChequeBottomMessageLogin}";
        }

       
    }
}
