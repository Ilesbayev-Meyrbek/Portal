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
            var user = await _userService.GetCurrentUser();

            if ( user.Role is {AdminForScale: true})
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
        return View(new User());
    }

    // POST: Role/Create
    [HttpPost]
    public async Task<ActionResult> CreateUser(User user)
    {
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

        return View(user);
    }

    public async Task<ActionResult> EditUser(int id)
    {
        var userResult = await _userService.GetAsync(id);

        if (!userResult.Success)
        {
            return NotFound();
        }

        var adminResult = await _adminService.GetAsync(User.Identity.Name);

        if (adminResult.Success)
        {
            var marketsResult = await _marketService.GetAllAsync();
            if(marketsResult.Success)
                ViewBag.MarketID = new SelectList(marketsResult.Data, "MarketID", "Name");
            var rolesResult = await _roleService.GetAllAsync(r => true);
            if (rolesResult.Success)
                ViewBag.RoleID = new SelectList(rolesResult.Data, "ID", "Name");
            
            ViewBag.Markets = new List<string> {userResult.Data.Market.MarketID, userResult.Data.Market.Name};
        }
        else
        {
            var user = await _userService.GetCurrentUser();

            if ( user.Role is {AdminForScale: true})
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
                
                ViewBag.Markets = new List<string> {userResult.Data.Market.MarketID, userResult.Data.Market.Name};
            }
        }

        //#region Log
        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Info("Изменение пользователя");
        //#endregion

        return View(userResult.Data);
    }

    // POST: Users/EditUser
    [HttpPost]
    public async Task<ActionResult> EditUser(User user)
    {
        Result<MarketsName>? marketResult;
        Result<List<Role>>? rolesResult;

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
            if(result.Success)
                return RedirectToAction("Users");
        }

        return NotFound();
    }
}