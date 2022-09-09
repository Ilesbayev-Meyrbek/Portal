using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph;
using Portal.Classes;
using Portal.Models;
using Portal.Services.Interfaces;
using User = Portal.Models.User;

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
        var currentUser = await _userService.GetCurrentUser();

        ViewBag.IsAdmin = currentUser.IsAdmin;

        var users = await _userService.GetUsersAsync(User.Identity.Name);

        #region Log

        new Logs.Logs(currentUser, "Users", "", "").WriteInfoLogs();

        #endregion

        return View(users.Data);
    }

    // GET: Role/Create
    public async Task<ActionResult> CreateUser()
    {
        var currentUser = await _userService.GetCurrentUser();

        if (currentUser.IsAdmin)
        {
            var marketsResult = await _marketService.GetAllAsync();
            if (marketsResult.Success)
                ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");

            var rolesResult = await _roleService.GetAllAsync(r => true);
            if (rolesResult.Success)
                ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");

            ViewBag.IsAdmin = currentUser.IsAdmin;
        }
        else
        {
            if (currentUser.Role is { AdminForScale: true })
            {
                var marketsResult = await _marketService.GetAllAsync();
                if (marketsResult.Success)
                    ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");

                var rolesResult = await _roleService.GetAllAsync(w =>
                        (w.AdminForScale || w.Scales || w.POSs && !w.CreateCashiers && !w.EditCashiers &&
                        !w.DeleteCashiers && !w.CreateLogo && !w.EditLogo && !w.DeleteLogo && !w.CreateSettings &&
                        !w.EditSettings && !w.DeleteSettings && !w.CreateKeyboard && !w.EditKeyboard &&
                        !w.DeleteKeyboard) && w.Name != "Admin");
                if (rolesResult.Success)
                    ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");

                ViewBag.IsAdmin = currentUser.IsAdmin;
            }
        }

        #region Log

        new Logs.Logs(currentUser, "CreateUser", "", "").WriteInfoLogs();

        #endregion

        return View(new User());
    }

    // POST: Role/Create
    [HttpPost]
    public async Task<ActionResult> CreateUser(User user)
    {
        var currentUser = await _userService.GetCurrentUser();

        if (user.IsAdmin)
            user.MarketID = "STSO";

        var result = await _userService.CreateAsync(user);

        if (result.Success)
        {
            return RedirectToAction("Users");
        }

        var marketsResult = await _marketService.GetAllAsync();
        if (marketsResult.Success)
            ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");

        var rolesResult = await _roleService.GetAllAsync(r => true);
        if (rolesResult.Success)
            ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");

        #region Log

        string data = "ID = " + user.ID + ";\n";
        data = data + "MarketID = " + user.MarketID + ";\n";
        data = data + "Name = " + user.Name + ";\n";
        data = data + "Login = " + user.Login + ";\n";
        data = data + "RoleID = " + user.RoleID + ";\n";
        data = data + "IsAdmin = " + user.IsAdmin + ";\n";

        new Logs.Logs(currentUser, "CreateUser", data, "").WriteInfoLogs();

        #endregion

        return View(user);
    }

    public async Task<ActionResult> EditUser(int id)
    {
        var currentUser = await _userService.GetCurrentUser();

        var userResult = await _userService.GetAsync(id);

        if (!userResult.Success)
        {
            return NotFound();
        }

        if (currentUser.IsAdmin)
        {
            var marketsResult = await _marketService.GetAllAsync();
            if (marketsResult.Success)
                ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");
            var rolesResult = await _roleService.GetAllAsync(r => true);
            if (rolesResult.Success)
                ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");

            ViewBag.Markets = new List<string> { userResult.Data.Market.MarketID, userResult.Data.Market.Name };

            ViewBag.IsAdmin = currentUser.IsAdmin;
        }
        else
        {
            if (currentUser.Role is { AdminForScale: true })
            {
                var marketsResult = await _marketService.GetAllAsync();
                if (marketsResult.Success)
                    ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");

                var rolesResult = await _roleService.GetAllAsync(w =>
                    (w.AdminForScale || w.Scales || w.POSs && !w.CreateCashiers && !w.EditCashiers &&
                    !w.DeleteCashiers && !w.CreateLogo && !w.EditLogo && !w.DeleteLogo && !w.CreateSettings &&
                    !w.EditSettings && !w.DeleteSettings && !w.CreateKeyboard && !w.EditKeyboard &&
                    !w.DeleteKeyboard) && w.Name != "Admin");
                if (rolesResult.Success)
                    ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");

                ViewBag.Markets = new List<string> { userResult.Data.Market.MarketID, userResult.Data.Market.Name };

                ViewBag.IsAdmin = currentUser.IsAdmin;
            }
        }

        #region Log

        new Logs.Logs(currentUser, "EditUser", "", "").WriteInfoLogs();

        #endregion

        return View(userResult.Data);
    }

    // POST: Users/EditUser
    [HttpPost]
    public async Task<ActionResult> EditUser(User user)
    {
        Result<MarketsName>? marketResult;
        Result<List<Role>>? rolesResult;

        if (user.IsAdmin)
            user.MarketID = "OFCE";

        var result = await _userService.EditAsync(user);

        if (result.Success)
        {
            return RedirectToAction("Users");
        }

        marketResult = await _marketService.GetAsync(user.MarketID);
        if (marketResult.Success)
            ViewBag.Markets = new List<string> { marketResult.Data.MarketID, marketResult.Data.Name };

        rolesResult = await _roleService.GetAllAsync(r => true);
        if (rolesResult.Success)
            ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");

        #region Log

        string data = "ID = " + user.ID + ";\n";
        data = data + "MarketID = " + user.MarketID + ";\n";
        data = data + "Name = " + user.Name + ";\n";
        data = data + "Login = " + user.Login + ";\n";
        data = data + "RoleID = " + user.RoleID + ";\n";
        data = data + "IsAdmin = " + user.IsAdmin + ";\n";
        var currentUser = await _userService.GetCurrentUser();
        new Logs.Logs(currentUser, "EditUser", data, "").WriteInfoLogs();

        #endregion

        return View(user);
    }

    [HttpGet]
    [ActionName("DeleteUser")]
    public async Task<IActionResult> ConfirmDeleteUser(int? id)
    {
        if (id.HasValue)
        {
            var userResult = await _userService.GetAsync(id.Value);
            if (userResult.Success)
            {
                ViewBag.CurrentMarket = userResult.Data.Market.Name;
                return View(userResult.Data);
            }
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int? id)
    {
        if (id.HasValue)
        {
            var result = await _userService.RemoveAsync(id.Value);
            if (result.Success)
            {
                #region Log
                
                var currentUser = await _userService.GetCurrentUser();
                new Logs.Logs(currentUser, "DeleteUser", "", "Удален!").WriteInfoLogs();

                #endregion

                return RedirectToAction("Users");
            }
        }

        return NotFound();
    }
}