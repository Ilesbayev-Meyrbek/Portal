using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Models;
using Portal.Services.Interfaces;

namespace Portal.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IAdminService _adminService;
    private readonly IMarketService _marketService;
    private readonly IRoleService _roleService;

    public UserController(IUserService userService, 
        IAdminService adminService,
        IMarketService marketService,
        IRoleService roleService)
    {
        _userService = userService;
        _adminService = adminService;
        _marketService = marketService;
        _roleService = roleService;
    }
    public async Task<ActionResult> Users()
    {
        var users = await _userService.GetUsersAsync(User.Identity.Name);

        return View(users.Data);
    }

    // GET: Role/Create
    public async Task<ActionResult> CreateUser()
    {
        var adminResult = await _adminService.GetAsync(User.Identity.Name);

        if (adminResult.Success)
        {
            var marketsResult = await _marketService.GetAllAsync();
            if(marketsResult.Success)
                ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");
            var rolesResult = await _roleService.GetAllAsync(r => true);
            if (rolesResult.Success)
                ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");
        }
        else
        {
            var user = await _userService.GetAsync(User.Identity.Name);

            if (user.Success && user.Data.Role is {AdminForScale: true})
            {
                var marketsResult = await _marketService.GetAllAsync();
                if(marketsResult.Success)
                    ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");

                var rolesResult = await _roleService.GetAllAsync(w =>
                        w.AdminForScale || w.Scales || w.POSs && !w.CreateCashiers && !w.EditCashiers &&
                        !w.DeleteCashiers && !w.CreateLogo && !w.EditLogo && !w.DeleteLogo && !w.CreateSettings &&
                        !w.EditSettings && !w.DeleteSettings && !w.CreateKeyboard && !w.EditKeyboard &&
                        !w.DeleteKeyboard);

                if(rolesResult.Success)
                    ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");
            }
        }

        //#region Log
        //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];
        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Info("Создание пользователя");
        //#endregion

        return View(new User());
    }

    // POST: Role/Create
    [HttpPost]
    public async Task<ActionResult> CreateUser(User user)
    {
        Result<List<MarketsName>>? marketsResult;
        Result<List<Role>>? rolesResult;
        if (ModelState.IsValid)
        {
            var result = await _userService.CreateAsync(user);

            if (result.Success)
            {
                return RedirectToAction("Users");
            }

            marketsResult = await _marketService.GetAllAsync();
            if(marketsResult.Success)
                ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");
            rolesResult = await _roleService.GetAllAsync(r => true);
            if (rolesResult.Success)
                ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");

            return View(user);
        }

        marketsResult = await _marketService.GetAllAsync();
        if(marketsResult.Success)
            ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");
        rolesResult = await _roleService.GetAllAsync(r => true);
        if (rolesResult.Success)
            ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");

        //#region Log
        //_data = "ID=" + user.ID.ToString() + ";\n";
        //_data = _data + "MarketID=" + user.MarketID + ";\n";
        //_data = _data + "Name=" + user.Name + ";\n";
        //_data = _data + "Login=" + user.Login + ";\n";
        //_data = _data + "RoleID=" + user.RoleID;
        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Ошибка при сохранение пользователя (не корректное заполнение полей)");
        //#endregion

        return View(user);
    }

    public async Task<ActionResult> EditUser(int? id)
    {
        User user = new Portal.DB.DB(_ctx, _sctx).GetUser(id);

        if (id == null)
        {
            return NotFound();
        }

        List<Role> roles = new List<Role>();
        MarketsName market = new MarketsName();
        List<MarketsName> markets = new List<MarketsName>();

        var admin = new Portal.DB.DB(_ctx, _sctx).GetAdmin(User.Identity.Name);

        if (admin != null)
        {
            markets = new Portal.DB.DB(_ctx, _sctx).GetMarkets();
            ViewBag.MarketID = new SelectList(markets, "MarketID", "Name");

            market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(user.MarketID);
            ViewBag.Markets = new List<string> {market.MarketID.ToString(), market.Name};

            roles = new Portal.DB.DB(_ctx, _sctx).GetRoles();
            ViewBag.RoleID = new SelectList(roles, "ID", "Name");
        }
        else
        {
            var _userRole = new Portal.DB.DB(_ctx, _sctx).GetUserRole(User.Identity.Name);

            if (_userRole.AdminForScale)
            {
                markets = new Portal.DB.DB(_ctx, _sctx).GetMarkets();
                ViewBag.MarketID = new SelectList(markets, "MarketID", "Name");

                market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(user.MarketID);
                ViewBag.Markets = new List<string> {market.MarketID.ToString(), market.Name};

                roles = new Portal.DB.DB(_ctx, _sctx).GetRoles().Where(w =>
                        w.AdminForScale || w.Scales || w.POSs && !w.CreateCashiers && !w.EditCashiers &&
                        !w.DeleteCashiers && !w.CreateLogo && !w.EditLogo && !w.DeleteLogo && !w.CreateSettings &&
                        !w.EditSettings && !w.DeleteSettings && !w.CreateKeyboard && !w.EditKeyboard &&
                        !w.DeleteKeyboard)
                    .ToList();
                ViewBag.RoleID = new SelectList(roles, "ID", "Name");
            }
        }

        //#region Log
        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Info("Изменение пользователя");
        //#endregion

        return View(user);
    }

    // POST: Users/EditUser
    [HttpPost]
    public async Task<ActionResult> EditUser(User user, string MarketID)
    {
        MarketsName market;

        //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];
        string _data = string.Empty;

        if (ModelState.IsValid)
        {
            var isEditing = new Portal.DB.DB(_ctx, _sctx).SaveEditUser(user);

            if (isEditing)
            {
                //#region Log
                //_data = "ID=" + user.ID.ToString() + ";\n";
                //_data = _data + "MarketID=" + user.MarketID + ";\n";
                //_data = _data + "Name=" + user.Name + ";\n";
                //_data = _data + "Login=" + user.Login + ";\n";
                //_data = _data + "RoleID=" + user.RoleID;
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Info("Сохранение изменение пользователя");
                //#endregion

                return RedirectToAction("Users");
            }
            else
            {
                market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(user.MarketID);
                ViewBag.Markets = new List<string> {market.MarketID.ToString(), market.Name};
                ViewBag.RoleID = new SelectList(new Portal.DB.DB(_ctx, _sctx).GetRoles(), "ID", "Name");

                //#region Log
                //_data = "ID=" + user.ID.ToString() + ";\n";
                //_data = _data + "MarketID=" + user.MarketID + ";\n";
                //_data = _data + "Name=" + user.Name + ";\n";
                //_data = _data + "Login=" + user.Login + ";\n";
                //_data = _data + "RoleID=" + user.RoleID;
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Ошибка при сохранение изменение пользователя");
                //#endregion

                return View(user);
            }
        }

        market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(user.MarketID);
        ViewBag.Markets = new List<string> {market.MarketID.ToString(), market.Name};
        ViewBag.RoleID = new SelectList(new Portal.DB.DB(_ctx, _sctx).GetRoles(), "ID", "Name");

        //#region Log
        //_data = "ID=" + user.ID.ToString() + ";\n";
        //_data = _data + "MarketID=" + user.MarketID + ";\n";
        //_data = _data + "Name=" + user.Name + ";\n";
        //_data = _data + "Login=" + user.Login + ";\n";
        //_data = _data + "RoleID=" + user.RoleID;
        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Ошибка при сохранение изменение пользователя (не корректное заполнение полей)");
        //#endregion

        return View(user);
    }

    [HttpGet]
    [ActionName("DeleteUser")]
    public async Task<IActionResult> ConfirmDeleteUser(int? id)
    {
        if (id != null)
        {
            User user = new Portal.DB.DB(_ctx, _sctx).GetUser(id);

            ViewBag.CurrentMarket = new Portal.DB.DB(_ctx, _sctx).GetMarkets(user.MarketID).Name;

            if (user != null)
                return View(user);
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int? id)
    {
        if (id != null)
        {
            var isDeleted = new Portal.DB.DB(_ctx, _sctx).DeleteUser(id);
            return RedirectToAction("Users");
        }

        return NotFound();
    }
}