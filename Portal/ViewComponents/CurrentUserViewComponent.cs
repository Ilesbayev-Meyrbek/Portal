using Microsoft.AspNetCore.Mvc;
using Portal.CacheManager;
using Portal.Classes;
using Portal.DB;
using Portal.Models;
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
            
            string keyCache = GetCacheKey(userLogin);
            if (_cacheManager.Get(keyCache) is CurrentUser currentUserCache)
            {
                return View("Default", currentUserCache);
            }
            CurrentUser currentUser = new CurrentUser();
            currentUser.Login = userLogin;

            var adminResult = await _adminService.GetAsync(userLogin);
            if (adminResult.Success)
            {
                currentUser.IsAdmin = true;
                currentUser.MarketID = "OFCE";
                currentUser.IsUser = false;
            }
            else
            {
                var userResult = await _userService.GetAsync(userLogin);
                if (userResult.Success)
                {
                    currentUser.MarketID = userResult.Data.MarketID;
                    currentUser.Roles = userResult.Data.Role;
                }

                currentUser.IsAdmin = false;
                currentUser.IsUser = true;
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
