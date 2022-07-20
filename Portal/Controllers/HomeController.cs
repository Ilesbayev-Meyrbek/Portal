using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Classes;
using Portal.DB;
using Portal.Models;
using System.Diagnostics;
using System.Net;

namespace Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DataContext _ctx;
        private readonly ScaleContext _sctx;

        public HomeController(ILogger<HomeController> logger, DataContext ctx, ScaleContext sctx)
        {
            _logger = logger;
            this._ctx = ctx;
            this._sctx = sctx;
        }

        public IActionResult Index()
        {
            //string marketForSession = new Portal.DB.DB(_ctx, _sctx).GetUserMarketID(User.Identity.Name);
            //CurrentUser _currentUser = new CurrentUser();
            //_currentUser.Login = User.Identity.Name;
            //_currentUser.MarketID = marketForSession;

            //this.Session["CurrentUser"] = _currentUser;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Roles
        
        public ActionResult Roles()
        {
            var roles = new Portal.DB.DB(_ctx, _sctx).GetRoles();

            return View(roles);
        }

        // GET: Role/Create
        public ActionResult CreateRole()
        {
            Role role = new Role();
            return View(role);
        }

        // POST: Role/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                var isCreated = new Portal.DB.DB(_ctx, _sctx).SaveNewRole(role);

                if (isCreated)
                    return RedirectToAction("Roles");
                else
                    return View(role);
            }

            return View(role);
        }

        public async Task<ActionResult> EditRole(int? id)
        {
            if (id != null)
            {
                var role = new Portal.DB.DB(_ctx, _sctx).GetRoleForEdit(id);

                if (role != null)
                    return View(role);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> EditRole(Role role)
        {
            if (ModelState.IsValid)
            {
                var isEdited = new Portal.DB.DB(_ctx, _sctx).SaveEditRole(role);

                if (isEdited)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    return View(role);
                }
            }

            return View(role);
        }

        [HttpGet]
        [ActionName("DeleteRole")]
        public async Task<IActionResult> ConfirmDeleteRole(int? id)
        {
            if (id != null)
            {
                Role role = new Portal.DB.DB(_ctx, _sctx).GetRoleForDelete(id);
                if (role != null)
                    return View(role);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(int? id)
        {
            if (id != null)
            {
                var isDeleted = new Portal.DB.DB(_ctx, _sctx).DeleteRole(id);
                return RedirectToAction("Roles");
            }
            return NotFound();
        }

        #endregion

        #region Users

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

            return View(users);
        }

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

                    roles = new Portal.DB.DB(_ctx, _sctx).GetRoles().Where(w => w.AdminForScale || w.Scales || w.POSs && !w.CreateCashiers && !w.EditCashiers && !w.DeleteCashiers && !w.CreateLogo && !w.EditLogo && !w.DeleteLogo && !w.CreateSettings && !w.EditSettings && !w.DeleteSettings && !w.CreateKeyboard && !w.EditKeyboard && !w.DeleteKeyboard).ToList();

                    ViewBag.RoleID = new SelectList(roles, "ID", "Name");
                }
            }

            //#region Log
            //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Info("Создание пользователя");
            //#endregion

            return View(user);
        }

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
                ViewBag.Markets = new List<string> { market.MarketID.ToString(), market.Name };

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
                    ViewBag.Markets = new List<string> { market.MarketID.ToString(), market.Name };

                    roles = new Portal.DB.DB(_ctx, _sctx).GetRoles().Where(w => w.AdminForScale || w.Scales || w.POSs && !w.CreateCashiers && !w.EditCashiers && !w.DeleteCashiers && !w.CreateLogo && !w.EditLogo && !w.DeleteLogo && !w.CreateSettings && !w.EditSettings && !w.DeleteSettings && !w.CreateKeyboard && !w.EditKeyboard && !w.DeleteKeyboard).ToList();
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
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(User user)
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
                    ViewBag.Markets = new List<string> { market.MarketID.ToString(), market.Name };
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
            ViewBag.Markets = new List<string> { market.MarketID.ToString(), market.Name };
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
















        #endregion












        #region Reports

        public ActionResult Reports()
        {
            var marketList = (_ctx.Chequeses.Select(x => x.MarketId)).Union(_ctx.NewCheques.Select(x => x.MarketId)).Distinct().ToArray();
            var posList = (_ctx.Chequeses.Select(x => x.Posnum)).Union(_ctx.NewCheques.Select(x => x.Posnum)).Distinct().ToArray();
            var termimalListList = (_ctx.Chequeses.Select(x => x.TerminalId)).Union(_ctx.NewCheques.Select(x => x.TerminalId)).Distinct().ToArray();

            Array.Sort(marketList);
            Array.Sort(posList);
            Array.Sort(termimalListList);

            ReportDatalistDto rdd = new ReportDatalistDto()
            {
                TerminalId = termimalListList,
                MarketId = marketList,
                Posnum = posList
            };


            return View(rdd);
        }

        #endregion

    }
}