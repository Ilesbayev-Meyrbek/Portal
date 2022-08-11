using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Models;
using Portal.Services.Interfaces;

namespace Portal.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    public ActionResult Users()
    {
        var users = new Portal.DB.DB(_ctx, _sctx).GetUsers(User.Identity.Name);

        //string marketForSession = new DB(db).GetUserMarketID(User.Identity.Name);
        //CurrentUser _currentUser = new CurrentUser();
        //_currentUser.Login = User.Identity.Name;
        //_currentUser.MarketID = marketForSession;

        //this.Session["CurrentUser"] = _currentUser;

        //#region Log
        //logger.WithProperty("MarketID", marketForSession).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Info("Пользователи");
        //#endregion

        return View(_userService.get);
    }

    // GET: Role/Create
    public ActionResult CreateUser()
    {
        User user = new User();
        List<Role> roles = new List<Role>();
        List<MarketsName> markets = new List<MarketsName>();

        var admin = new Portal.DB.DB(_ctx, _sctx).GetAdmin(User.Identity.Name);

        if (admin != null)
        {
            markets = new Portal.DB.DB(_ctx, _sctx).GetMarkets();
            ViewBag.MarketID = new SelectList(markets, "MarketID", "Name");
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
        //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];
        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Info("Создание пользователя");
        //#endregion

        return View(user);
    }

    // POST: Role/Create
    [HttpPost]
    public async Task<ActionResult> CreateUser(User user)
    {
        List<MarketsName> markets;
        List<Role> roles;

        //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];
        //string _data = string.Empty;

        if (ModelState.IsValid)
        {
            var isCreated = new Portal.DB.DB(_ctx, _sctx).SaveNewUser(user);

            if (isCreated)
            {
                //#region Log
                //_data = "ID=" + user.ID.ToString() + ";\n";
                //_data = _data + "MarketID=" + user.MarketID + ";\n";
                //_data = _data + "Name=" + user.Name + ";\n";
                //_data = _data + "Login=" + user.Login + ";\n";
                //_data = _data + "RoleID=" + user.RoleID;
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Info("Сохранение пользователя");
                //#endregion

                return RedirectToAction("Users");
            }
            else
            {
                markets = new Portal.DB.DB(_ctx, _sctx).GetMarkets();
                ViewBag.MarketID = new SelectList(markets, "MarketID", "Name");

                roles = new Portal.DB.DB(_ctx, _sctx).GetRoles();
                ViewBag.RoleID = new SelectList(roles, "ID", "Name");

                //#region Log
                //_data = "ID=" + user.ID.ToString() + ";\n";
                //_data = _data + "MarketID=" + user.MarketID + ";\n";
                //_data = _data + "Name=" + user.Name + ";\n";
                //_data = _data + "Login=" + user.Login + ";\n";
                //_data = _data + "RoleID=" + user.RoleID;
                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Ошибка при сохранение пользователя");
                //#endregion

                return View(user);
            }
        }

        markets = new Portal.DB.DB(_ctx, _sctx).GetMarkets();
        ViewBag.MarketID = new SelectList(markets, "MarketID", "Name");

        roles = new Portal.DB.DB(_ctx, _sctx).GetRoles();
        ViewBag.RoleID = new SelectList(roles, "ID", "Name");

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
        //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];

        User user = new Portal.DB.DB(_ctx, _sctx).GetUser(id);

        if (id == null)
        {
            //    //#region Log
            //    //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Error("Выбранный пользователь не найден - HttpNotFound");
            //    //#endregion

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