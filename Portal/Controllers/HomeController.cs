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