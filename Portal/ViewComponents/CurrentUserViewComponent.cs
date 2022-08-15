using Microsoft.AspNetCore.Mvc;
using Portal.Classes;
using Portal.DB;
using Portal.Models;

namespace Portal.ViewComponents
{
    public class CurrentUserViewComponent : ViewComponent
    {
        private readonly DataContext _ctx;
        private readonly ScaleContext _sctx;

        public CurrentUserViewComponent(DataContext ctx, ScaleContext sctx)
        {
            _ctx = ctx;
            _sctx = sctx;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            User _user = null;
            Role _role = null;
            var _admin = new DB.DB(_ctx, _sctx).GetAdmin(User.Identity.Name);
            if (_admin == null)
            {
                _user = new DB.DB(_ctx, _sctx).CheckUser(User.Identity.Name);
                _role = new DB.DB(_ctx, _sctx).GetUserRole(User.Identity.Name);
            }

            CurrentUser currentUser = new CurrentUser();
            currentUser.Login = User.Identity.Name;
            
            if (_admin != null)
            { 
                currentUser.IsAdmin = true;
                currentUser.MarketID = "OFCE";
            }
            else 
            {
                currentUser.IsAdmin = false;
            }

            if (_user != null)
            {
                currentUser.IsUser = true;
                string mID = new Portal.DB.DB(_ctx, _sctx).GetUserMarketID(User.Identity.Name);
                currentUser.MarketID = mID;
            }
            else
            {
                currentUser.IsUser = false;
            }

            if (_role != null) currentUser.Roles = _role;

            return View("Default", currentUser);
        }
    }
}
