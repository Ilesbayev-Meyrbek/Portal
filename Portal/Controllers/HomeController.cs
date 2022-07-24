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

        #endregion

        #region Cashiers

        public ActionResult Cashiers(string MarketID)
        {
            var cashiers = new Portal.DB.DB(_ctx, _sctx).GetCashiers(User.Identity.Name, MarketID);

            cashiers.Markets.Insert(0, new MarketsName
            {
                MarketID = "",
                Name = "Выберите магазин",
                POS = "",
                POSVersion = "",
                FilesLoaded = false
            });

            ViewBag.MarketID = new SelectList(cashiers.Markets != null ? cashiers.Markets : new List<MarketsName>(), "MarketID", "Name");

            //#region Log
            //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("Data", "").Info("Кассиры");
            //#endregion

            string marketForSession = new Portal.DB.DB(_ctx, _sctx).GetUserMarketID(User.Identity.Name);
            CurrentUser _currentUser = new CurrentUser();
            _currentUser.Login = User.Identity.Name;
            _currentUser.MarketID = marketForSession;

            //this.Session["CurrentUser"] = _currentUser;

            //#region Log
            //logger.WithProperty("MarketID", marketForSession).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Info("Кассиры");
            //#endregion

            return View(cashiers);
        }

        // GET: Cashiers/CreateCashier
        public ActionResult CreateCashier()
        {
            Cashier cashier = new Cashier();

            var markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            //#region Log
            //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", "").Info("Создание кассира");
            //#endregion

            return View(cashier);
        }

        // POST: Cashiers/CreateCashier
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCashier(Cashier cashier, FormCollection form, string[] SelectedMarkets, string MarketForUser)
        {
            List<MarketsName> markets = new List<MarketsName>();

            //CurrentUser _currentUser = (CurrentUser)this.Session["CurrentUser"];
            //string _data = string.Empty;

            if (ModelState.IsValid)
            {
                if (SelectedMarkets != null)
                {
                    if (cashier.ID.Length > 5 && cashier.ID.Length < 26)
                    {
                        if (IsDigitsOnly(cashier.ID))
                        {
                            for (int i = 0; i < SelectedMarkets.Length; i++)
                            {
                                var checkCashierID = new Portal.DB.DB(_ctx, _sctx).GetCashier(cashier.ID, SelectedMarkets[i]);

                                if (checkCashierID == null)
                                {
                                    if (SelectedMarkets[i] != "All")
                                    {
                                        Cashier _cashier = new Cashier();
                                        _cashier.ID = cashier.ID;
                                        _cashier.CashierName = cashier.CashierName;
                                        _cashier.IsAdmin = cashier.IsAdmin;
                                        _cashier.IsDiscounter = cashier.IsDiscounter;
                                        _cashier.Password = "";
                                        _cashier.TabelNumber = string.Empty;
                                        _cashier.DateBegin = DateTime.Now;
                                        _cashier.DateEnd = DateTime.Now;
                                        _cashier.IsGoodDisco = false;
                                        _cashier.IsInvoicer = false;
                                        _cashier.IsSaved = false;
                                        _cashier.IsSavedToPOS = 0;
                                        _cashier.IsSavedToMarket = "0";
                                        _cashier.MarketID = SelectedMarkets[i];

                                        var isSaved = new Portal.DB.DB(_ctx, _sctx).SaveNewCashier(SelectedMarkets[i], _cashier);

                                        //#region Log
                                        //_data = string.Empty;
                                        //_data = "ID = " + cashier.ID + ";\n";
                                        //_data = _data + "TabelNumber = \"\";\n";
                                        //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                        //_data = _data + "Password = \"\";\n";
                                        //_data = _data + "DateBegin = " + DateTime.Now + ";\n";
                                        //_data = _data + "DateEnd = " + DateTime.Now + ";\n";
                                        //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                        //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                        //_data = _data + "IsGoodDisco = false;\n";
                                        //_data = _data + "IsInvoicer = false;\n";
                                        //_data = _data + "IsSaved = false;\n";
                                        //_data = _data + "IsSavedToPOS = 0;\n";
                                        //_data = _data + "IsSavedToMarket = 0;\n";
                                        //_data = _data + "MarketID = " + SelectedMarkets[i] + ";\n";
                                        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Info("Сохранение кассира");
                                        //#endregion
                                    }
                                }
                                else
                                {
                                    markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
                                    ViewBag.Markets = markets;
                                    ViewBag.MarketsCount = markets.Count;

                                    //#region Log
                                    //_data = string.Empty;
                                    //_data = "ID = " + cashier.ID + ";\n";
                                    //_data = _data + "TabelNumber = \"\";\n";
                                    //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                    //_data = _data + "Password = \"\";\n";
                                    //_data = _data + "DateBegin = " + DateTime.Now + "\n";
                                    //_data = _data + "DateEnd = " + DateTime.Now + "\n";
                                    //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                    //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                    //_data = _data + "IsGoodDisco = false;\n";
                                    //_data = _data + "IsInvoicer = false;\n";
                                    //_data = _data + "IsSaved = false;\n";
                                    //_data = _data + "IsSavedToPOS = 0;\n";
                                    //_data = _data + "IsSavedToMarket = 0;\n";
                                    //_data = _data + "MarketID = " + cashier.MarketID + ";\n";
                                    //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Пользователь с таким именем или паролем уже существует");
                                    //#endregion

                                    TempData["msg"] = "<script>alert('Пользователь с таким именем или паролем уже существует!');</script>";
                                    return View(cashier);
                                }
                            }

                            return RedirectToAction("Cashiers");
                        }
                        else
                        {
                            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
                            ViewBag.Markets = markets;
                            ViewBag.MarketsCount = markets.Count;

                            //#region Log
                            //_data = string.Empty;
                            //_data = "ID = " + cashier.ID + ";\n";
                            //_data = _data + "TabelNumber = \"\";\n";
                            //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
                            //_data = _data + "Password = \"\";\n";
                            //_data = _data + "DateBegin = " + DateTime.Now + "\n";
                            //_data = _data + "DateEnd = " + DateTime.Now + "\n";
                            //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                            //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                            //_data = _data + "IsGoodDisco = false;\n";
                            //_data = _data + "IsInvoicer = false;\n";
                            //_data = _data + "IsSaved = false;\n";
                            //_data = _data + "IsSavedToPOS = 0;\n";
                            //_data = _data + "IsSavedToMarket = 0;\n";
                            //_data = _data + "MarketID = " + cashier.MarketID + ";\n";
                            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Поле «Пароль» должен содержать только цифры");
                            //#endregion

                            TempData["msg"] = "<script>alert('Поле «Пароль» должен содержать только цифры!');</script>";
                            return View(cashier);
                        }
                    }
                    else
                    {
                        markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
                        ViewBag.Markets = markets;
                        ViewBag.MarketsCount = markets.Count;

                        //#region Log
                        //_data = string.Empty;
                        //_data = "ID = " + cashier.ID + ";\n";
                        //_data = _data + "TabelNumber = \"\";\n";
                        //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
                        //_data = _data + "Password = \"\";\n";
                        //_data = _data + "DateBegin = " + DateTime.Now + "\n";
                        //_data = _data + "DateEnd = " + DateTime.Now + "\n";
                        //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                        //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                        //_data = _data + "IsGoodDisco = false;\n";
                        //_data = _data + "IsInvoicer = false;\n";
                        //_data = _data + "IsSaved = false;\n";
                        //_data = _data + "IsSavedToPOS = 0;\n";
                        //_data = _data + "IsSavedToMarket = 0;\n";
                        //_data = _data + "MarketID = " + cashier.MarketID + ";\n";
                        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Длина пароля должна быть не меньше 6 и не больше 25 символов");
                        //#endregion

                        TempData["msg"] = "<script>alert('Длина пароля должна быть не меньше 6 и не больше 25 символов!');</script>";
                        return View(cashier);
                    }
                }
                else
                {
                    if (cashier.ID.Length > 5 && cashier.ID.Length < 26)
                    {
                        markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
                        string mm = markets[0].MarketID;

                        var checkCashierID = new Portal.DB.DB(_ctx, _sctx).GetCashier(cashier.ID, mm);

                        if (checkCashierID == null)
                        {
                            if (IsDigitsOnly(cashier.ID))
                            {
                                cashier.Password = "";
                                cashier.TabelNumber = string.Empty;
                                cashier.DateBegin = DateTime.Now;
                                cashier.DateEnd = DateTime.Now;
                                cashier.IsGoodDisco = false;
                                cashier.IsInvoicer = false;
                                cashier.IsSaved = false;
                                cashier.IsSavedToPOS = 0;
                                cashier.IsSavedToMarket = "0";
                                cashier.MarketID = mm;

                                var isSaved = new Portal.DB.DB(_ctx, _sctx).SaveNewCashier(mm, cashier);

                                //#region Log
                                //_data = string.Empty;
                                //_data = "ID = " + cashier.ID + ";\n";
                                //_data = _data + "TabelNumber = \"\";\n";
                                //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                //_data = _data + "Password = \"\";\n";
                                //_data = _data + "DateBegin = " + DateTime.Now + "\n";
                                //_data = _data + "DateEnd = " + DateTime.Now + "\n";
                                //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                //_data = _data + "IsGoodDisco = false;\n";
                                //_data = _data + "IsInvoicer = false;\n";
                                //_data = _data + "IsSaved = false;\n";
                                //_data = _data + "IsSavedToPOS = 0;\n";
                                //_data = _data + "IsSavedToMarket = 0;\n";
                                //_data = _data + "MarketID = " + cashier.MarketID + ";\n";
                                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Info("Сохранение кассира");
                                //#endregion

                                return RedirectToAction("Cashiers");
                            }
                            else
                            {
                                ViewBag.Markets = markets;
                                ViewBag.MarketsCount = markets.Count;

                                //#region Log
                                //_data = string.Empty;
                                //_data = "ID = " + cashier.ID + ";\n";
                                //_data = _data + "TabelNumber = \"\";\n";
                                //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                //_data = _data + "Password = \"\";\n";
                                //_data = _data + "DateBegin = " + DateTime.Now + "\n";
                                //_data = _data + "DateEnd = " + DateTime.Now + "\n";
                                //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                //_data = _data + "IsGoodDisco = false;\n";
                                //_data = _data + "IsInvoicer = false;\n";
                                //_data = _data + "IsSaved = false;\n";
                                //_data = _data + "IsSavedToPOS = 0;\n";
                                //_data = _data + "IsSavedToMarket = 0;\n";
                                //_data = _data + "MarketID = " + cashier.MarketID + ";\n";
                                //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Поле «Пароль» должен содержать только цифры");
                                //#endregion

                                TempData["msg"] = "<script>alert('Поле «Пароль» должен содержать только цифры!');</script>";
                                return View(cashier);
                            }
                        }
                        else
                        {
                            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
                            ViewBag.Markets = markets;
                            ViewBag.MarketsCount = markets.Count;

                            //#region Log
                            //_data = string.Empty;
                            //_data = "ID = " + cashier.ID + ";\n";
                            //_data = _data + "TabelNumber = \"\";\n";
                            //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
                            //_data = _data + "Password = \"\";\n";
                            //_data = _data + "DateBegin = " + DateTime.Now + "\n";
                            //_data = _data + "DateEnd = " + DateTime.Now + "\n";
                            //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                            //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                            //_data = _data + "IsGoodDisco = false;\n";
                            //_data = _data + "IsInvoicer = false;\n";
                            //_data = _data + "IsSaved = false;\n";
                            //_data = _data + "IsSavedToPOS = 0;\n";
                            //_data = _data + "IsSavedToMarket = 0;\n";
                            //_data = _data + "MarketID = " + cashier.MarketID + ";\n";
                            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Пользователь с таким номером уже существует");
                            //#endregion

                            TempData["msg"] = "<script>alert('Пользователь с таким номером уже существует!');</script>";
                            return View(cashier);
                        }
                    }
                    else
                    {
                        markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
                        ViewBag.Markets = markets;
                        ViewBag.MarketsCount = markets.Count;

                        //#region Log
                        //_data = string.Empty;
                        //_data = "ID = " + cashier.ID + ";\n";
                        //_data = _data + "TabelNumber = \"\";\n";
                        //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
                        //_data = _data + "Password = \"\";\n";
                        //_data = _data + "DateBegin = " + DateTime.Now + "\n";
                        //_data = _data + "DateEnd = " + DateTime.Now + "\n";
                        //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                        //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                        //_data = _data + "IsGoodDisco = false;\n";
                        //_data = _data + "IsInvoicer = false;\n";
                        //_data = _data + "IsSaved = false;\n";
                        //_data = _data + "IsSavedToPOS = 0;\n";
                        //_data = _data + "IsSavedToMarket = 0;\n";
                        //_data = _data + "MarketID = " + cashier.MarketID + ";\n";
                        //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Длина пароля должна быть не меньше 6 и не больше 25 символов");
                        //#endregion

                        TempData["msg"] = "<script>alert('Длина пароля должна быть не меньше 6 и не больше 25 символов!');</script>";
                        return View(cashier);
                    }
                }
            }

            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            //#region Log
            //_data = string.Empty;
            //_data = "ID = " + cashier.ID + ";\n";
            //_data = _data + "TabelNumber = \"\";\n";
            //_data = _data + "CashierName = " + cashier.CashierName + ";\n";
            //_data = _data + "Password = \"\";\n";
            //_data = _data + "DateBegin = " + DateTime.Now + "\n";
            //_data = _data + "DateEnd = " + DateTime.Now + "\n";
            //_data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
            //_data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
            //_data = _data + "IsGoodDisco = false;\n";
            //_data = _data + "IsInvoicer = false;\n";
            //_data = _data + "IsSaved = false;\n";
            //_data = _data + "IsSavedToPOS = 0;\n";
            //_data = _data + "IsSavedToMarket = 0;\n";
            //_data = _data + "MarketID = " + cashier.MarketID + ";\n";
            //logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("IdentityUser", User.Identity.Name).WithProperty("Data", _data).Error("Некорректное заполнение полей");
            //#endregion

            TempData["msg"] = "<script>alert('Некорректное заполнение полей!');</script>";
            return View(cashier);
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public ActionResult EditCashier(string id, string cashierName, string marketID)
        {
            Cashier cashier = new Portal.DB.DB(_ctx, _sctx).GetCashier(id, marketID);
            //db.Cashiers.Where(w => w.ID == id && w.CashierName == cashierName && w.MarketID == marketID).FirstOrDefault();//await db.Cashiers.FindAsync(id);

            if (cashier == null)
                return NotFound();

            var markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            return View(cashier);
        }

        // POST: Cashiers/EditCashier
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCashier(Cashier cashier, string cashierID, string idPre, string namePre)
        {
            List<MarketsName> markets;

            if (ModelState.IsValid)
            {
                if (cashier.ID.Length > 5 && cashier.ID.Length < 26)
                {
                    try
                    {
                        new Portal.DB.DB(_ctx, _sctx).EditCashier(cashier);

                        //var checkCashierID = new DB(db).GetCashier(cashier.ID, cashier.MarketID);

                        //if(checkCashierID == null)
                        //{
                        //    Cashier _cashier = new Cashier();
                        //    _cashier.ID = cashier.ID;
                        //    _cashier.CashierName = cashier.CashierName;
                        //    _cashier.Password = "";
                        //    _cashier.IsAdmin = cashier.IsAdmin;
                        //    _cashier.IsDiscounter = cashier.IsDiscounter;
                        //    _cashier.TabelNumber = string.Empty;
                        //    _cashier.DateBegin = DateTime.Now;
                        //    _cashier.DateEnd = DateTime.Now;
                        //    _cashier.IsGoodDisco = false;
                        //    _cashier.IsInvoicer = false;
                        //    _cashier.IsSaved = false;
                        //    _cashier.IsSavedToPOS = 0;
                        //    _cashier.IsSavedToMarket = "0";
                        //    _cashier.MarketID = cashier.MarketID;

                        //    db.Cashiers.Add(_cashier);
                        //    await db.SaveChangesAsync();
                        //}

                        return RedirectToAction("Cashiers");
                    }
                    catch (Exception)
                    {
                        ///return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
                    ViewBag.Markets = markets;
                    ViewBag.MarketsCount = markets.Count;

                    TempData["msg"] = "<script>alert('Длина пароля должно быть не меньше 6 и не больше 25 символов!');</script>";
                    return View(cashier);
                }
            }

            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            return View(cashier);
        }

        [HttpGet]
        [ActionName("DeleteCashier")]
        public async Task<IActionResult> ConfirmDeleteCashier(int? id)
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
        public async Task<IActionResult> DeleteCashier(int? id)
        {
            if (id != null)
            {
                var isDeleted = new Portal.DB.DB(_ctx, _sctx).DeleteUser(id);
                return RedirectToAction("Cashiers");
            }
            return NotFound();
        }

        #endregion

        #region Keyboards

        public ActionResult Keyboards(string MarketID)
        {
            var keyboards = new Portal.DB.DB(_ctx, _sctx).GetKeyboards(User.Identity.Name, MarketID);

            if (string.IsNullOrEmpty(MarketID))
                ViewBag.SelectText = false;
            else
                ViewBag.SelectText = true;

            keyboards.Markets.Insert(0, new MarketsName
            {
                MarketID = "",
                Name = "Выберите магазин",
                POS = "",
                POSVersion = "",
                FilesLoaded = false
            });

            ViewBag.MarketID = new SelectList(keyboards.Markets != null ? keyboards.Markets : new List<MarketsName>(), "MarketID", "Name");

            TempData["CurrentMarketID"] = MarketID;

            return View(keyboards);
        }

        // GET: Keyboards/CreateKeyboard
        public ActionResult CreateKeyboard()
        {
            Keyboard keyboard = new Keyboard();

            var currentMarketID = TempData.Peek("CurrentMarketID");

            ViewBag.MarketID = currentMarketID;
            ViewBag.SettingsKey = new Portal.DB.DB(_ctx, _sctx).GetKeys();

            return View(keyboard);
        }

        // POST: Keyboards/CreateKeyboard
        [HttpPost]
        public async Task<ActionResult> CreateKeyboard(Keyboard keyboard, string[] btnkeyH, FormCollection form, string MarketForUser)
        {
            List<string> valLst = new List<string>();

            var settingsKey = new Portal.DB.DB(_ctx, _sctx).GetKeys();

            for (int i = 0; i < btnkeyH.Length; i++)
            {
                string value = btnkeyH[i].ToString().Split(':')[1].Trim().Replace("\n", "");

                if (value.Length > 2)
                {
                    var keyCode = new Portal.DB.DB(_ctx, _sctx).GetKeyCode(value);

                    if (keyCode != null && keyCode.Count > 0)
                    {
                        var str = string.Format("{0}:{1}", btnkeyH[i].ToString().Split(':')[0].Trim(), keyCode[0].KeyCode);
                        valLst.Add(str);
                    }
                    else if (IsDigitsOnly(value))
                    {
                        var str = string.Format("{0}:{1}", btnkeyH[i].ToString().Split(':')[0].Trim(), value);
                        valLst.Add(str);
                    }
                }
            }

            if (ModelState.IsValid && valLst.Count > 0)
            {
                for (int i = 0; i < valLst.Count; i++)
                {
                    string strID = valLst[i].ToString().Split(':')[0];
                    string strValue = valLst[i].ToString().Split(':')[1].Trim().Replace("\n", "");

                    if (!string.IsNullOrEmpty(strID) && strID == "btn1H") keyboard.Key_1 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn2H") keyboard.Key_2 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn3H") keyboard.Key_3 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn4H") keyboard.Key_4 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn5H") keyboard.Key_5 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn6H") keyboard.Key_6 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn7H") keyboard.Key_7 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn8H") keyboard.Key_8 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn9H") keyboard.Key_9 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn10H") keyboard.Key_10 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn11H") keyboard.Key_11 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn12H") keyboard.Key_12 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn13H") keyboard.Key_13 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn14H") keyboard.Key_14 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn15H") keyboard.Key_15 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn16H") keyboard.Key_16 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn17H") keyboard.Key_17 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn30H") keyboard.Key_30 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn31H") keyboard.Key_31 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn32H") keyboard.Key_32 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn33H") keyboard.Key_33 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn34H") keyboard.Key_34 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn35H") keyboard.Key_35 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn36H") keyboard.Key_36 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn37H") keyboard.Key_37 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn38H") keyboard.Key_38 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn39H") keyboard.Key_39 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn40H") keyboard.Key_40 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn41H") keyboard.Key_41 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn42H") keyboard.Key_42 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn43H") keyboard.Key_43 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn44H") keyboard.Key_44 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn45H") keyboard.Key_45 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn46H") keyboard.Key_46 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn47H") keyboard.Key_47 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn48H") keyboard.Key_48 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn49H") keyboard.Key_49 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn50H") keyboard.Key_50 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn51H") keyboard.Key_51 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn52H") keyboard.Key_52 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn53H") keyboard.Key_53 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn54H") keyboard.Key_54 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn55H") keyboard.Key_55 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn56H") keyboard.Key_56 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn57H") keyboard.Key_57 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn58H") keyboard.Key_58 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn59H") keyboard.Key_59 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn60H") keyboard.Key_60 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn61H") keyboard.Key_61 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn62H") keyboard.Key_62 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn63H") keyboard.Key_63 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn64H") keyboard.Key_64 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn65H") keyboard.Key_65 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn66H") keyboard.Key_66 = strValue;
                    if (!string.IsNullOrEmpty(strID) && strID == "btn67H") keyboard.Key_67 = strValue;
                }

                keyboard.MarketID = MarketForUser;
                keyboard.Pos_num = 0;
                keyboard.IsSavedToPOS = 0;
                keyboard.IsSaved = false;

                var isSaved = new Portal.DB.DB(_ctx, _sctx).SaveNewKeyboard(MarketForUser, keyboard);

                return RedirectToAction("Keyboards");
            }

            var markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(User.Identity.Name);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;
            ViewBag.SettingsKey = settingsKey;

            TempData["msg"] = "<script>alert('Не корректное заполнение полей!');</script>";

            return View(keyboard);
        }

        public Keyboard ReplaceKeysValue(Keyboard sk, List<SettingsKey> settingsKeys)
        {
            var sk1 = settingsKeys.Where(w => w.KeyCode == sk.Key_1).ToList();
            if (sk1.Count > 0) sk.Key_1 = sk1[0].Value;
            var sk2 = settingsKeys.Where(w => w.KeyCode == sk.Key_2).ToList();
            if (sk2.Count > 0) sk.Key_2 = sk2[0].Value;
            var sk3 = settingsKeys.Where(w => w.KeyCode == sk.Key_3).ToList();
            if (sk3.Count > 0) sk.Key_3 = sk3[0].Value;
            var sk4 = settingsKeys.Where(w => w.KeyCode == sk.Key_4).ToList();
            if (sk4.Count > 0) sk.Key_4 = sk4[0].Value;
            var sk5 = settingsKeys.Where(w => w.KeyCode == sk.Key_5).ToList();
            if (sk5.Count > 0) sk.Key_5 = sk5[0].Value;
            var sk6 = settingsKeys.Where(w => w.KeyCode == sk.Key_6).ToList();
            if (sk6.Count > 0) sk.Key_6 = sk6[0].Value;
            var sk7 = settingsKeys.Where(w => w.KeyCode == sk.Key_7).ToList();
            if (sk7.Count > 0) sk.Key_7 = sk7[0].Value;
            var sk8 = settingsKeys.Where(w => w.KeyCode == sk.Key_8).ToList();
            if (sk8.Count > 0) sk.Key_8 = sk8[0].Value;
            var sk9 = settingsKeys.Where(w => w.KeyCode == sk.Key_9).ToList();
            if (sk9.Count > 0) sk.Key_9 = sk9[0].Value;
            var sk10 = settingsKeys.Where(w => w.KeyCode == sk.Key_10).ToList();
            if (sk10.Count > 0) sk.Key_10 = sk10[0].Value;

            var sk11 = settingsKeys.Where(w => w.KeyCode == sk.Key_11).ToList();
            if (sk11.Count > 0) sk.Key_11 = sk11[0].Value;
            var sk12 = settingsKeys.Where(w => w.KeyCode == sk.Key_12).ToList();
            if (sk12.Count > 0) sk.Key_12 = sk12[0].Value;
            var sk13 = settingsKeys.Where(w => w.KeyCode == sk.Key_13).ToList();
            if (sk13.Count > 0) sk.Key_13 = sk13[0].Value;
            var sk14 = settingsKeys.Where(w => w.KeyCode == sk.Key_14).ToList();
            if (sk14.Count > 0) sk.Key_14 = sk14[0].Value;
            var sk15 = settingsKeys.Where(w => w.KeyCode == sk.Key_15).ToList();
            if (sk15.Count > 0) sk.Key_15 = sk15[0].Value;
            var sk16 = settingsKeys.Where(w => w.KeyCode == sk.Key_16).ToList();
            if (sk16.Count > 0) sk.Key_16 = sk16[0].Value;
            var sk17 = settingsKeys.Where(w => w.KeyCode == sk.Key_17).ToList();
            if (sk17.Count > 0) sk.Key_17 = sk17[0].Value;

            var sk30 = settingsKeys.Where(w => w.KeyCode == sk.Key_30).ToList();
            if (sk30.Count > 0) sk.Key_30 = sk30[0].Value;
            var sk31 = settingsKeys.Where(w => w.KeyCode == sk.Key_31).ToList();
            if (sk31.Count > 0) sk.Key_31 = sk31[0].Value;
            var sk32 = settingsKeys.Where(w => w.KeyCode == sk.Key_32).ToList();
            if (sk32.Count > 0) sk.Key_32 = sk32[0].Value;
            var sk33 = settingsKeys.Where(w => w.KeyCode == sk.Key_33).ToList();
            if (sk33.Count > 0) sk.Key_33 = sk33[0].Value;
            var sk34 = settingsKeys.Where(w => w.KeyCode == sk.Key_34).ToList();
            if (sk34.Count > 0) sk.Key_34 = sk34[0].Value;
            var sk35 = settingsKeys.Where(w => w.KeyCode == sk.Key_35).ToList();
            if (sk35.Count > 0) sk.Key_35 = sk35[0].Value;
            var sk36 = settingsKeys.Where(w => w.KeyCode == sk.Key_36).ToList();
            if (sk36.Count > 0) sk.Key_36 = sk36[0].Value;
            var sk37 = settingsKeys.Where(w => w.KeyCode == sk.Key_37).ToList();
            if (sk37.Count > 0) sk.Key_37 = sk37[0].Value;
            var sk38 = settingsKeys.Where(w => w.KeyCode == sk.Key_38).ToList();
            if (sk38.Count > 0) sk.Key_38 = sk38[0].Value;
            var sk39 = settingsKeys.Where(w => w.KeyCode == sk.Key_39).ToList();
            if (sk39.Count > 0) sk.Key_39 = sk39[0].Value;

            var sk40 = settingsKeys.Where(w => w.KeyCode == sk.Key_40).ToList();
            if (sk40.Count > 0) sk.Key_40 = sk40[0].Value;
            var sk41 = settingsKeys.Where(w => w.KeyCode == sk.Key_41).ToList();
            if (sk41.Count > 0) sk.Key_41 = sk41[0].Value;
            var sk42 = settingsKeys.Where(w => w.KeyCode == sk.Key_42).ToList();
            if (sk42.Count > 0) sk.Key_42 = sk42[0].Value;
            var sk43 = settingsKeys.Where(w => w.KeyCode == sk.Key_43).ToList();
            if (sk43.Count > 0) sk.Key_43 = sk43[0].Value;
            var sk44 = settingsKeys.Where(w => w.KeyCode == sk.Key_44).ToList();
            if (sk44.Count > 0) sk.Key_44 = sk44[0].Value;
            var sk45 = settingsKeys.Where(w => w.KeyCode == sk.Key_45).ToList();
            if (sk45.Count > 0) sk.Key_45 = sk45[0].Value;
            var sk46 = settingsKeys.Where(w => w.KeyCode == sk.Key_46).ToList();
            if (sk46.Count > 0) sk.Key_46 = sk46[0].Value;
            var sk47 = settingsKeys.Where(w => w.KeyCode == sk.Key_47).ToList();
            if (sk47.Count > 0) sk.Key_47 = sk47[0].Value;
            var sk48 = settingsKeys.Where(w => w.KeyCode == sk.Key_48).ToList();
            if (sk48.Count > 0) sk.Key_48 = sk48[0].Value;
            var sk49 = settingsKeys.Where(w => w.KeyCode == sk.Key_49).ToList();
            if (sk49.Count > 0) sk.Key_49 = sk49[0].Value;

            var sk50 = settingsKeys.Where(w => w.KeyCode == sk.Key_50).ToList();
            if (sk50.Count > 0) sk.Key_50 = sk50[0].Value;
            var sk51 = settingsKeys.Where(w => w.KeyCode == sk.Key_51).ToList();
            if (sk51.Count > 0) sk.Key_51 = sk51[0].Value;
            var sk52 = settingsKeys.Where(w => w.KeyCode == sk.Key_52).ToList();
            if (sk52.Count > 0) sk.Key_52 = sk52[0].Value;
            var sk53 = settingsKeys.Where(w => w.KeyCode == sk.Key_53).ToList();
            if (sk53.Count > 0) sk.Key_53 = sk53[0].Value;
            var sk54 = settingsKeys.Where(w => w.KeyCode == sk.Key_54).ToList();
            if (sk54.Count > 0) sk.Key_54 = sk54[0].Value;
            var sk55 = settingsKeys.Where(w => w.KeyCode == sk.Key_55).ToList();
            if (sk55.Count > 0) sk.Key_55 = sk55[0].Value;
            var sk56 = settingsKeys.Where(w => w.KeyCode == sk.Key_56).ToList();
            if (sk56.Count > 0) sk.Key_56 = sk56[0].Value;
            var sk57 = settingsKeys.Where(w => w.KeyCode == sk.Key_57).ToList();
            if (sk57.Count > 0) sk.Key_57 = sk57[0].Value;
            var sk58 = settingsKeys.Where(w => w.KeyCode == sk.Key_58).ToList();
            if (sk58.Count > 0) sk.Key_58 = sk58[0].Value;
            var sk59 = settingsKeys.Where(w => w.KeyCode == sk.Key_59).ToList();
            if (sk59.Count > 0) sk.Key_59 = sk59[0].Value;

            var sk60 = settingsKeys.Where(w => w.KeyCode == sk.Key_60).ToList();
            if (sk60.Count > 0) sk.Key_60 = sk60[0].Value;
            var sk61 = settingsKeys.Where(w => w.KeyCode == sk.Key_61).ToList();
            if (sk61.Count > 0) sk.Key_61 = sk61[0].Value;
            var sk62 = settingsKeys.Where(w => w.KeyCode == sk.Key_62).ToList();
            if (sk62.Count > 0) sk.Key_62 = sk62[0].Value;
            var sk63 = settingsKeys.Where(w => w.KeyCode == sk.Key_63).ToList();
            if (sk63.Count > 0) sk.Key_63 = sk63[0].Value;
            var sk64 = settingsKeys.Where(w => w.KeyCode == sk.Key_64).ToList();
            if (sk64.Count > 0) sk.Key_64 = sk64[0].Value;
            var sk65 = settingsKeys.Where(w => w.KeyCode == sk.Key_65).ToList();
            if (sk65.Count > 0) sk.Key_65 = sk65[0].Value;
            var sk66 = settingsKeys.Where(w => w.KeyCode == sk.Key_66).ToList();
            if (sk66.Count > 0) sk.Key_66 = sk66[0].Value;
            var sk67 = settingsKeys.Where(w => w.KeyCode == sk.Key_67).ToList();
            if (sk67.Count > 0) sk.Key_67 = sk67[0].Value;

            return sk;
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