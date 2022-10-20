using Portal.CacheManager;
using Microsoft.AspNetCore.Mvc;
using Portal.Services.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.ViewComponents
{
    public class CurrentUserViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly ICacheManager _cacheManager;

        public CurrentUserViewComponent(IUserService userService, ICacheManager cacheManager)
        {
            _userService = userService;
            _cacheManager = cacheManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userLogin = User.Identity.Name;

            var currentUser = await _userService.GetCurrentUser();

            string keyCache = GetCacheKey(userLogin);

            if (_cacheManager.Get(keyCache) is Users _currentUser)
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
