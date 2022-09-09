using Portal.Models;
using Portal.CacheManager;
using Microsoft.AspNetCore.Mvc;
using Portal.Services.Interfaces;

namespace Portal.ViewComponents
{
    public class CurrentUserViewComponent : ViewComponent
    {
        private readonly IAdminService _adminService;
        private readonly IUserService _userService;
        private readonly ICacheManager _cacheManager;

        public CurrentUserViewComponent(IAdminService adminService, IUserService userService, ICacheManager cacheManager)
        {
            _adminService = adminService;
            _userService = userService;
            _cacheManager = cacheManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userLogin = User.Identity.Name;

            var currentUser = await _userService.GetCurrentUser();

            string keyCache = GetCacheKey(userLogin);

            if (_cacheManager.Get(keyCache) is User _currentUser)
            {
                return View("Default", _currentUser);
            }

            _cacheManager.Set(currentUser, keyCache);
            return View("Default", currentUser);
        }
        
        private string GetCacheKey(string userLogin)
        {
            return $"GetCurrentUserByLogin_{userLogin}";
        }
    }
}
