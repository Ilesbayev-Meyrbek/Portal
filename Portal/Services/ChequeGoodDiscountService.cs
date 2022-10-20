using Portal.CacheManager;
using Portal.Repositories.Interfaces;
using Portal.Services.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Services
{
    public class ChequeGoodDiscountService : IChequeGoodDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChequeGoodDiscountService> _logger;
        private readonly IRoleService _roleService;
        private readonly ICacheManager _cacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChequeGoodDiscountService(IUnitOfWork unitOfWork,
            IRoleService roleService,
            ICacheManager cacheManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChequeGoodDiscountService> logger)
        {
            _unitOfWork = unitOfWork;
            _roleService = roleService;
            _cacheManager = cacheManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
    }
}
