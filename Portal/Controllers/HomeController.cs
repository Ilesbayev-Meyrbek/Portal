using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Fluent;
using NuGet.Packaging.Core;
using Portal.Classes;
using Portal.DB;
using Portal.Logs;
using Portal.Models;
using Portal.Services.Interfaces;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;

namespace Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ILogger<HomeController> _logger;

        private readonly DataContext _ctx;
        private readonly ScaleContext _sctx;

        private readonly IUserService _userService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger, DataContext ctx, ScaleContext sctx, IUserService userService)
        {
            this._httpContextAccessor = httpContextAccessor;
            _logger = logger;
            this._ctx = ctx;
            this._sctx = sctx;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userService.GetCurrentUser();

            #region Log

            new Logs.Logs(currentUser, "Index", "", "").WriteInfoLogs();

            #endregion

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            #region Log

            User currentUser = await _userService.GetCurrentUser();
            new Logs.Logs(currentUser, "Error", "", HttpContext.TraceIdentifier.ToString()).WriteErrorLogs();

            #endregion
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Roles

        public async Task<ActionResult> Roles()
        {
            User currentUser = await _userService.GetCurrentUser();

            var roles = new Portal.DB.DB(_ctx, _sctx).GetRoles(currentUser);

            #region Log
            new Logs.Logs(currentUser, "Roles", "", "").WriteInfoLogs();
            #endregion

            return View(roles);
        }

        // GET: Role/Create
        public async Task<ActionResult> CreateRole()
        {
            Role role = new Role();

            #region Log

            User currentUser = await _userService.GetCurrentUser();
            new Logs.Logs(currentUser, "CreateRole", "", "").WriteInfoLogs();

            #endregion

            return View(role);
        }

        // POST: Role/Create
        [HttpPost]
        public async Task<ActionResult> CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                User currentUser = await _userService.GetCurrentUser();

                var isCreated = new Portal.DB.DB(_ctx, _sctx).SaveNewRole(role, currentUser);

                #region Log
                string data = "ID = " + role.ID + ";\n";
                data = data + "Name = " + role.Name + ";\n";
                data = data + "CreateCashiers = " + role.CreateCashiers + ";\n";
                data = data + "EditCashiers = " + role.EditCashiers + ";\n";
                data = data + "DeleteCashiers = " + role.DeleteCashiers + ";\n";

                data = data + "CreateLogo = " + role.CreateLogo + ";\n";
                data = data + "EditLogo = " + role.EditLogo + ";\n";
                data = data + "DeleteLogo = " + role.DeleteLogo + ";\n";

                data = data + "CreateKeyboard = " + role.CreateKeyboard + ";\n";
                data = data + "EditKeyboard = " + role.EditKeyboard + ";\n";
                data = data + "DeleteKeyboard = " + role.DeleteKeyboard + ";\n";

                data = data + "AllMarkets = " + role.AllMarkets + ";\n";
                data = data + "AdminForScale = " + role.AdminForScale + ";\n";
                data = data + "Scales = " + role.Scales + ";\n";
                data = data + "POSs = " + role.POSs + ";\n";

                
                new Logs.Logs(currentUser, "CreateRole", data, "Создан!").WriteInfoLogs();

                #endregion

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
                User currentUser = await _userService.GetCurrentUser();

                var role = new Portal.DB.DB(_ctx, _sctx).GetRoleForEdit(id, currentUser);

                #region Log
                new Logs.Logs(currentUser, "EditRole", "", "").WriteInfoLogs();
                #endregion

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
                User currentUser = await _userService.GetCurrentUser();

                var isEdited = new Portal.DB.DB(_ctx, _sctx).SaveEditRole(role, currentUser);

                if (isEdited)
                {
                    #region Log
                    string data = "ID = " + role.ID + ";\n";
                    data = data + "Name = " + role.Name + ";\n";
                    data = data + "CreateCashiers = " + role.CreateCashiers + ";\n";
                    data = data + "EditCashiers = " + role.EditCashiers + ";\n";
                    data = data + "DeleteCashiers = " + role.DeleteCashiers + ";\n";

                    data = data + "CreateLogo = " + role.CreateLogo + ";\n";
                    data = data + "EditLogo = " + role.EditLogo + ";\n";
                    data = data + "DeleteLogo = " + role.DeleteLogo + ";\n";

                    data = data + "CreateKeyboard = " + role.CreateKeyboard + ";\n";
                    data = data + "EditKeyboard = " + role.EditKeyboard + ";\n";
                    data = data + "DeleteKeyboard = " + role.DeleteKeyboard + ";\n";

                    data = data + "AllMarkets = " + role.AllMarkets + ";\n";
                    data = data + "AdminForScale = " + role.AdminForScale + ";\n";
                    data = data + "Scales = " + role.Scales + ";\n";
                    data = data + "POSs = " + role.POSs + ";\n";
                    
                    new Logs.Logs(currentUser, "CreateRole", data, "Изменен!").WriteInfoLogs();

                    #endregion

                    return RedirectToAction("Roles");
                }
                else
                {
                    #region Log

                    string data = "ID = " + role.ID + ";\n";
                    data = data + "Name = " + role.Name + ";\n";
                    data = data + "CreateCashiers = " + role.CreateCashiers + ";\n";
                    data = data + "EditCashiers = " + role.EditCashiers + ";\n";
                    data = data + "DeleteCashiers = " + role.DeleteCashiers + ";\n";

                    data = data + "CreateLogo = " + role.CreateLogo + ";\n";
                    data = data + "EditLogo = " + role.EditLogo + ";\n";
                    data = data + "DeleteLogo = " + role.DeleteLogo + ";\n";

                    data = data + "CreateKeyboard = " + role.CreateKeyboard + ";\n";
                    data = data + "EditKeyboard = " + role.EditKeyboard + ";\n";
                    data = data + "DeleteKeyboard = " + role.DeleteKeyboard + ";\n";

                    data = data + "AllMarkets = " + role.AllMarkets + ";\n";
                    data = data + "AdminForScale = " + role.AdminForScale + ";\n";
                    data = data + "Scales = " + role.Scales + ";\n";
                    data = data + "POSs = " + role.POSs + ";\n";

                    currentUser = await _userService.GetCurrentUser();
                    new Logs.Logs(currentUser, "CreateRole", data, "Не изменено!").WriteInfoLogs();

                    #endregion

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
                User currentUser = await _userService.GetCurrentUser();
                Role role = new Portal.DB.DB(_ctx, _sctx).GetRoleForDelete(id, currentUser);
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
                User currentUser = await _userService.GetCurrentUser();

                var isDeleted = new Portal.DB.DB(_ctx, _sctx).DeleteRole(id, currentUser);

                #region Log
                
                new Logs.Logs(currentUser, "DeleteRole", "", "Удален!").WriteInfoLogs();

                #endregion

                return RedirectToAction("Roles");
            }
            return NotFound();
        }

        #endregion

        #region Logo

        public ActionResult Logos(string MarketID)
        {
            var currentUser = _userService.GetCurrentUser().Result;

            #region Log

            new Logs.Logs(currentUser, "Logos", "", "").WriteInfoLogs();

            #endregion

            var logos = new Portal.DB.DB(_ctx, _sctx).GetLogos(currentUser, MarketID);
            ViewBag.MarketID = new SelectList(logos.Markets != null ? logos.Markets : new List<MarketsName>(), "MarketID", "Name");

            return View(logos);
        }

        // GET: Logos/CreateLogo
        public ActionResult CreateLogo()
        {
            var currentUser = _userService.GetCurrentUser().Result;

            Logo logo = new Logo();
            var markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            #region Log

            new Logs.Logs(currentUser, "CreateLogo", "", "").WriteInfoLogs();

            #endregion

            return View(logo);
        }

        // POST: Logos/CreateLogo
        [HttpPost]
        public async Task<ActionResult> CreateLogo(LogoViewModel lvm, string[] SelectedMarkets)
        {
            Logo logo = new Logo();

            var currentUser = _userService.GetCurrentUser().Result;

            List<MarketsName> markets;

            if (SelectedMarkets.Length > 0)
            {
                if (lvm.BMP != null)
                {
                    if (lvm.DateS <= lvm.DateE)
                    {
                        byte[] imageData = new byte[0];

                        if (lvm.BMP != null)
                        {
                            using (BinaryReader br = new BinaryReader(lvm.BMP.OpenReadStream()))
                            {
                                imageData = br.ReadBytes((int)lvm.BMP.Length);
                            }
                        }

                        MemoryStream ms = new MemoryStream(imageData);
                        Image pic = Image.FromStream(ms);
                        var bitDepth = pic.PixelFormat.ToString();
                        var imgWidth = pic.Width;
                        var imgHeight = pic.Height;

                        if (lvm.BMP.ContentType == "image/bmp" && imgWidth == 500 && bitDepth == "Format1bppIndexed")
                        {
                            var ds = lvm.DateS.ToString("yyyyMMdd");
                            var de = lvm.DateE.ToString("yyyyMMdd");

                            logo.DateS = lvm.DateS;
                            logo.DateE = lvm.DateE;
                            logo.DateBegin = Convert.ToInt32(ds);
                            logo.DateEnd = Convert.ToInt32(de);
                            logo.BMP = imageData;
                            logo.Note = lvm.Note;
                            logo.IsSaved = false;
                            logo.IsSavedToPOS = 0;

                            foreach (var item in SelectedMarkets)
                            {
                                if (item != "All")
                                {
                                    bool IsDeleteOldLogos = new Portal.DB.DB(_ctx, _sctx).DeleteOldLogos(item, lvm, currentUser);
                                    bool isSaved = new Portal.DB.DB(_ctx, _sctx).SaveNewLogo(item, lvm, ds, de, imageData, currentUser);

                                    #region Log

                                    string data = "MarketID = " + logo.MarketID + ";\n";
                                    if (logo.BMP == null)
                                        data = data + "BMP = ;\n";
                                    else
                                        data = data + "BMP = " + logo.BMP.Length + ";\n";
                                    data = data + "DateBegin = " + logo.DateBegin + ";\n";
                                    data = data + "DateEnd = " + logo.DateEnd + ";\n";
                                    data = data + "Note = " + logo.Note + ";\n";
                                    data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                                    data = data + "IsSaved = " + logo.IsSaved + ";\n";
                                    data = data + "DateS = " + logo.DateS + ";\n";
                                    data = data + "DateE = " + logo.DateE + ";\n";
                                    new Logs.Logs(currentUser, "CreateLogo", data, "Добавлен!").WriteInfoLogs();

                                    #endregion
                                }
                            }

                            return RedirectToAction("Logos");
                        }
                        else
                        {
                            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                            ViewBag.Markets = markets;
                            ViewBag.MarketsCount = markets.Count;

                            TempData["msg"] = "<script>alert('Выбранный логотип не корректный, для выгрузки логотипа используйте картинки с размерами: 500х94, 500х250, 500х400, 500х510, 500х579, глубина цвета - 1 bpp и расширение BMP');</script>";

                            #region Log

                            string data = "MarketID = " + logo.MarketID + ";\n";
                            if (logo.BMP == null)
                                data = data + "BMP = ;\n";
                            else
                                data = data + "BMP = " + logo.BMP.Length + ";\n";
                            data = data + "DateBegin = " + logo.DateBegin + ";\n";
                            data = data + "DateEnd = " + logo.DateEnd + ";\n";
                            data = data + "Note = " + logo.Note + ";\n";
                            data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                            data = data + "IsSaved = " + logo.IsSaved + ";\n";
                            data = data + "DateS = " + logo.DateS + ";\n";
                            data = data + "DateE = " + logo.DateE + ";\n";

                            new Logs.Logs(currentUser, "CreateLogo", data, "Выбранный логотип не корректный, для выгрузки логотипа используйте картинки с размерами: 500х94, 500х250, 500х400, 500х510, 500х579, глубина цвета - 1 bpp и расширение BMP").WriteErrorLogs();

                            #endregion

                            return View(logo);
                        }
                    }
                    else
                    {
                        markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                        ViewBag.Markets = markets;
                        ViewBag.MarketsCount = markets.Count;

                        TempData["msg"] = "<script>alert('Дата начало должно быть меньше чем дата окончание действие логотипа');</script>";

                        #region Log

                        string data = "MarketID = " + logo.MarketID + ";\n";
                        if (logo.BMP == null)
                            data = data + "BMP = ;\n";
                        else
                            data = data + "BMP = " + logo.BMP.Length + ";\n";
                        data = data + "DateBegin = " + logo.DateBegin + ";\n";
                        data = data + "DateEnd = " + logo.DateEnd + ";\n";
                        data = data + "Note = " + logo.Note + ";\n";
                        data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                        data = data + "IsSaved = " + logo.IsSaved + ";\n";
                        data = data + "DateS = " + logo.DateS + ";\n";
                        data = data + "DateE = " + logo.DateE + ";\n";

                        new Logs.Logs(currentUser, "CreateLogo", data, "Дата начало должно быть меньше чем дата окончание действие логотипа").WriteErrorLogs();

                        #endregion

                        return View(logo);
                    }
                }
                else
                {
                    markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                    ViewBag.Markets = markets;
                    ViewBag.MarketsCount = markets.Count;

                    TempData["msg"] = "<script>alert('Не выбран файл логотипа');</script>";

                    #region Log

                    string data = "MarketID = " + logo.MarketID + ";\n";
                    if (logo.BMP == null)
                        data = data + "BMP = ;\n";
                    else
                        data = data + "BMP = " + logo.BMP.Length + ";\n";
                    data = data + "DateBegin = " + logo.DateBegin + ";\n";
                    data = data + "DateEnd = " + logo.DateEnd + ";\n";
                    data = data + "Note = " + logo.Note + ";\n";
                    data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                    data = data + "IsSaved = " + logo.IsSaved + ";\n";
                    data = data + "DateS = " + logo.DateS + ";\n";
                    data = data + "DateE = " + logo.DateE + ";\n";

                    new Logs.Logs(currentUser, "CreateLogo", data, "Не выбран файл логотипа").WriteErrorLogs();

                    #endregion

                    return View(logo);
                }
            }
            else
            {
                if (currentUser.IsAdmin)
                {
                    markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                    ViewBag.Markets = markets;
                    ViewBag.MarketsCount = markets.Count;

                    TempData["msg"] = "<script>alert('Не выбран маркет для выгрузки логотипа');</script>";

                    #region Log
                    string data = "MarketID = " + logo.MarketID + ";\n";
                    if (logo.BMP == null)
                        data = data + "BMP = ;\n";
                    else
                        data = data + "BMP = " + logo.BMP.Length + ";\n";
                    data = data + "DateBegin = " + logo.DateBegin + ";\n";
                    data = data + "DateEnd = " + logo.DateEnd + ";\n";
                    data = data + "Note = " + logo.Note + ";\n";
                    data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                    data = data + "IsSaved = " + logo.IsSaved + ";\n";
                    data = data + "DateS = " + logo.DateS + ";\n";
                    data = data + "DateE = " + logo.DateE + ";\n";
                    new Logs.Logs(currentUser, "CreateLogo", data, "Не выбран маркет для выгрузки логотипа").WriteErrorLogs();

                    #endregion

                    return View(logo);
                }
                else
                {
                    if (lvm.BMP != null)
                    {
                        if (lvm.DateS <= lvm.DateE)
                        {
                            byte[] imageData = new byte[0];

                            if (lvm.BMP != null)
                            {
                                using (BinaryReader br = new BinaryReader(lvm.BMP.OpenReadStream()))
                                {
                                    imageData = br.ReadBytes((int)lvm.BMP.Length);
                                }
                            }

                            MemoryStream ms = new MemoryStream(imageData);
                            Image pic = Image.FromStream(ms);
                            var bitDepth = pic.PixelFormat.ToString();
                            var imgWidth = pic.Width;
                            var imgHeight = pic.Height;

                            if (lvm.BMP.ContentType == "image/bmp" && imgWidth == 500 && bitDepth == "Format1bppIndexed")
                            {
                                var ds = lvm.DateS.ToString("yyyyMMdd");
                                var de = lvm.DateE.ToString("yyyyMMdd");

                                logo.DateS = lvm.DateS;
                                logo.DateE = lvm.DateE;
                                logo.DateBegin = Convert.ToInt32(ds);
                                logo.DateEnd = Convert.ToInt32(de);
                                logo.BMP = imageData;
                                logo.IsSaved = false;
                                logo.IsSavedToPOS = 0;
                                var _mID = new Portal.DB.DB(_ctx, _sctx).GetMarketForUser(User.Identity.Name, currentUser);
                                logo.Note = lvm.Note;
                                logo.MarketID = _mID;

                                var isEdited = new Portal.DB.DB(_ctx, _sctx).EditOldLogos(logo, currentUser);

                                #region Log

                                string data = "MarketID = " + logo.MarketID + ";\n";
                                if (logo.BMP == null)
                                    data = data + "BMP = ;\n";
                                else
                                    data = data + "BMP = " + logo.BMP.Length + ";\n";
                                data = data + "DateBegin = " + logo.DateBegin + ";\n";
                                data = data + "DateEnd = " + logo.DateEnd + ";\n";
                                data = data + "Note = " + logo.Note + ";\n";
                                data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                                data = data + "IsSaved = " + logo.IsSaved + ";\n";
                                data = data + "DateS = " + logo.DateS + ";\n";
                                data = data + "DateE = " + logo.DateE + ";\n";
                                new Logs.Logs(currentUser, "CreateLogo", data, "Добавлен!").WriteInfoLogs();

                                #endregion

                                return RedirectToAction("Logos");
                            }
                            else
                            {
                                markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                                ViewBag.Markets = markets;
                                ViewBag.MarketsCount = markets.Count;

                                TempData["msg"] = "<script>alert('Выбранный логотип не корректный, для выгрузки логотипа используйте картинки с размерами: 500х94, 500х250, 500х400, 500х510, 500х579, глубина цвета - 1 bpp и расширение BMP');</script>";

                                #region Log

                                string data = "MarketID = " + logo.MarketID + ";\n";
                                if (logo.BMP == null)
                                    data = data + "BMP = ;\n";
                                else
                                    data = data + "BMP = " + logo.BMP.Length + ";\n";
                                data = data + "DateBegin = " + logo.DateBegin + ";\n";
                                data = data + "DateEnd = " + logo.DateEnd + ";\n";
                                data = data + "Note = " + logo.Note + ";\n";
                                data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                                data = data + "IsSaved = " + logo.IsSaved + ";\n";
                                data = data + "DateS = " + logo.DateS + ";\n";
                                data = data + "DateE = " + logo.DateE + ";\n";
                                new Logs.Logs(currentUser, "CreateLogo", data, "Выбранный логотип не корректный, для выгрузки логотипа используйте картинки с размерами: 500х94, 500х250, 500х400, 500х510, 500х579, глубина цвета - 1 bpp и расширение BMP").WriteErrorLogs();

                                #endregion

                                return View(logo);
                            }
                        }
                        else
                        {
                            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                            ViewBag.Markets = markets;
                            ViewBag.MarketsCount = markets.Count;

                            TempData["msg"] = "<script>alert('Дата начало должно быть меньше чем дата окончание действие логотипа');</script>";

                            #region Log

                            string data = "MarketID = " + logo.MarketID + ";\n";
                            if (logo.BMP == null)
                                data = data + "BMP = ;\n";
                            else
                                data = data + "BMP = " + logo.BMP.Length + ";\n";
                            data = data + "DateBegin = " + logo.DateBegin + ";\n";
                            data = data + "DateEnd = " + logo.DateEnd + ";\n";
                            data = data + "Note = " + logo.Note + ";\n";
                            data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                            data = data + "IsSaved = " + logo.IsSaved + ";\n";
                            data = data + "DateS = " + logo.DateS + ";\n";
                            data = data + "DateE = " + logo.DateE + ";\n";
                            new Logs.Logs(currentUser, "CreateLogo", data, "Дата начало должно быть меньше чем дата окончание действие логотипа").WriteErrorLogs();

                            #endregion

                            return View(logo);
                        }
                    }
                    else
                    {
                        markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                        ViewBag.Markets = markets;
                        ViewBag.MarketsCount = markets.Count;

                        TempData["msg"] = "<script>alert('Не выбран файл логотипа');</script>";

                        #region Log

                        string data = "MarketID = " + logo.MarketID + ";\n";
                        if (logo.BMP == null)
                            data = data + "BMP = ;\n";
                        else
                            data = data + "BMP = " + logo.BMP.Length + ";\n";
                        data = data + "DateBegin = " + logo.DateBegin + ";\n";
                        data = data + "DateEnd = " + logo.DateEnd + ";\n";
                        data = data + "Note = " + logo.Note + ";\n";
                        data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                        data = data + "IsSaved = " + logo.IsSaved + ";\n";
                        data = data + "DateS = " + logo.DateS + ";\n";
                        data = data + "DateE = " + logo.DateE + ";\n";
                        new Logs.Logs(currentUser, "CreateLogo", data, "Не выбран файл логотипа").WriteErrorLogs();

                        #endregion

                        return View(logo);
                    }
                }
            }
        }

        public async Task<ActionResult> EditLogo(int? id)
        {
            if (id == null)
                return NotFound();

            User currentUser = await _userService.GetCurrentUser();

            Logo logo = new Portal.DB.DB(_ctx, _sctx).GetLogo(id, currentUser);
            
            var market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(logo.MarketID, currentUser);
            ViewBag.Market = market.Name;

            #region Log
            new Logs.Logs(currentUser, "EditLogo", "", "").WriteInfoLogs();
            #endregion

            return View(logo);
        }

        // POST: Logos/Edit
        [HttpPost]
        public async Task<ActionResult> EditLogo(LogoViewModel lvm, Logo logo)
        {
            MarketsName market;

            var ds = lvm.DateS.ToString("yyyyMMdd");
            var de = lvm.DateE.ToString("yyyyMMdd");

            logo.DateS = lvm.DateS;
            logo.DateE = lvm.DateE;
            logo.DateBegin = Convert.ToInt32(ds);
            logo.DateEnd = Convert.ToInt32(de);
            logo.Note = lvm.Note;
            logo.IsSaved = false;
            logo.IsSavedToPOS = 0;
            logo.MarketID = lvm.MarketID;

            byte[] imageData = new byte[0];

            User currentUser = await _userService.GetCurrentUser();

            if (lvm.BMP != null)
            {
                if (lvm.DateS <= lvm.DateE)
                {
                    if (lvm.BMP != null)
                    {
                        using (BinaryReader br = new BinaryReader(lvm.BMP.OpenReadStream()))
                        {
                            imageData = br.ReadBytes((int)lvm.BMP.Length);
                        }
                    }

                    MemoryStream ms = new MemoryStream(imageData);
                    Image pic = Image.FromStream(ms);
                    var bitDepth = pic.PixelFormat.ToString();
                    var imgWidth = pic.Width;
                    var imgHeight = pic.Height;

                    if (lvm.BMP.ContentType == "image/bmp" && imgWidth == 500 && bitDepth == "Format1bppIndexed")
                    {
                        logo.BMP = imageData;
                        var isEdited = new Portal.DB.DB(_ctx, _sctx).SaveEditLogo(logo, currentUser);

                        #region Log

                        string data = "MarketID = " + logo.MarketID + ";\n";
                        if (logo.BMP == null)
                            data = data + "BMP = ;\n";
                        else
                            data = data + "BMP = " + logo.BMP.Length + ";\n";
                        data = data + "DateBegin = " + logo.DateBegin + ";\n";
                        data = data + "DateEnd = " + logo.DateEnd + ";\n";
                        data = data + "Note = " + logo.Note + ";\n";
                        data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                        data = data + "IsSaved = " + logo.IsSaved + ";\n";
                        data = data + "DateS = " + logo.DateS + ";\n";
                        data = data + "DateE = " + logo.DateE + ";\n";

                        new Logs.Logs(currentUser, "EditLogo", data, "Изменен!").WriteInfoLogs();

                        #endregion

                        return RedirectToAction("Logos");
                    }
                    else
                    {
                        market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(logo.MarketID, currentUser);
                        ViewBag.Market = market.Name;

                        TempData["msg"] = "<script>alert('Выбранный файл логотипа не корректный, для выгрузки логотипа используйте картинки с размерами: 500х94, 500х250, 500х400, 500х510, 500х579, глубина цвета - 1 bpp и расширение BMP');</script>";

                        #region Log

                        string data = "MarketID = " + logo.MarketID + ";\n";
                        if (logo.BMP == null)
                            data = data + "BMP = ;\n";
                        else
                            data = data + "BMP = " + logo.BMP.Length + ";\n";
                        data = data + "DateBegin = " + logo.DateBegin + ";\n";
                        data = data + "DateEnd = " + logo.DateEnd + ";\n";
                        data = data + "Note = " + logo.Note + ";\n";
                        data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                        data = data + "IsSaved = " + logo.IsSaved + ";\n";
                        data = data + "DateS = " + logo.DateS + ";\n";
                        data = data + "DateE = " + logo.DateE + ";\n";
                        new Logs.Logs(currentUser, "EditLogo", data, "Выбранный файл логотипа не корректный, для выгрузки логотипа используйте картинки с размерами: 500х94, 500х250, 500х400, 500х510, 500х579, глубина цвета - 1 bpp и расширение BMP").WriteErrorLogs();

                        #endregion

                        return View(logo);
                    }
                }
                else
                {
                    market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(logo.MarketID, currentUser);
                    ViewBag.Market = market.Name;

                    TempData["msg"] = "<script>alert('Дата начало должно быть меньше чем дата окончание действие логотипа');</script>";

                    logo.BMP = imageData;

                    #region Log

                    string data = "MarketID = " + logo.MarketID + ";\n";
                    if (logo.BMP == null)
                        data = data + "BMP = ;\n";
                    else
                        data = data + "BMP = " + logo.BMP.Length + ";\n";
                    data = data + "DateBegin = " + logo.DateBegin + ";\n";
                    data = data + "DateEnd = " + logo.DateEnd + ";\n";
                    data = data + "Note = " + logo.Note + ";\n";
                    data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                    data = data + "IsSaved = " + logo.IsSaved + ";\n";
                    data = data + "DateS = " + logo.DateS + ";\n";
                    data = data + "DateE = " + logo.DateE + ";\n";
                    new Logs.Logs(currentUser, "EditLogo", data, "Дата начало должно быть меньше чем дата окончание действие логотипа").WriteErrorLogs();

                    #endregion


                    return View(logo);
                }
            }
            else if (lvm.BMP == null && logo.BMP != null)
            {
                imageData = logo.BMP;

                if (lvm.DateS <= lvm.DateE)
                {
                    MemoryStream ms = new MemoryStream(imageData);
                    Image pic = Image.FromStream(ms);
                    var bitDepth = pic.PixelFormat.ToString();
                    var imgWidth = pic.Width;
                    var imgHeight = pic.Height;
                    var imgFormat = new FileContentResult(imageData, "image/bmp");

                    if (imgFormat.ContentType == "image/bmp" && imgWidth == 500 && bitDepth == "Format1bppIndexed")
                    {
                        logo.BMP = imageData;
                        var isEdited = new Portal.DB.DB(_ctx, _sctx).SaveEditLogo(logo, currentUser);

                        #region Log

                        string data = "MarketID = " + logo.MarketID + ";\n";
                        if (logo.BMP == null)
                            data = data + "BMP = ;\n";
                        else
                            data = data + "BMP = " + logo.BMP.Length + ";\n";
                        data = data + "DateBegin = " + logo.DateBegin + ";\n";
                        data = data + "DateEnd = " + logo.DateEnd + ";\n";
                        data = data + "Note = " + logo.Note + ";\n";
                        data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                        data = data + "IsSaved = " + logo.IsSaved + ";\n";
                        data = data + "DateS = " + logo.DateS + ";\n";
                        data = data + "DateE = " + logo.DateE + ";\n";

                        new Logs.Logs(currentUser, "EditLogo", data, "Изменен!").WriteInfoLogs();

                        #endregion

                        return RedirectToAction("Logos");
                    }
                    else
                    {
                        market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(logo.MarketID, currentUser);
                        ViewBag.Market = market.Name;

                        TempData["msg"] = "<script>alert('Выбранный файл логотипа не корректный, для выгрузки логотипа используйте картинки с размерами: 500х94, 500х250, 500х400, 500х510, 500х579, глубина цвета - 1 bpp и расширение BMP');</script>";

                        #region Log

                        string data = "MarketID = " + logo.MarketID + ";\n";
                        if (logo.BMP == null)
                            data = data + "BMP = ;\n";
                        else
                            data = data + "BMP = " + logo.BMP.Length + ";\n";
                        data = data + "DateBegin = " + logo.DateBegin + ";\n";
                        data = data + "DateEnd = " + logo.DateEnd + ";\n";
                        data = data + "Note = " + logo.Note + ";\n";
                        data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                        data = data + "IsSaved = " + logo.IsSaved + ";\n";
                        data = data + "DateS = " + logo.DateS + ";\n";
                        data = data + "DateE = " + logo.DateE + ";\n";
                        new Logs.Logs(currentUser, "EditLogo", data, "Выбранный файл логотипа не корректный,для выгрузки логотипа используйте картинки с размерами: 500х94, 500х250, 500х400, 500х510, 500х579, глубина цвета - 1 bpp и расширение BMP").WriteErrorLogs();

                        #endregion

                        return View(logo);
                    }
                }
                else
                {
                    market = new Portal.DB.DB(_ctx, _sctx).GetMarkets(logo.MarketID, currentUser);
                    ViewBag.Market = market.Name;

                    TempData["msg"] = "<script>alert('Дата начало должно быть меньше чем дата окончание действие логотипа');</script>";

                    #region Log
                    string data = "MarketID = " + logo.MarketID + ";\n";
                    if (logo.BMP == null)
                        data = data + "BMP = ;\n";
                    else
                        data = data + "BMP = " + logo.BMP.Length + ";\n";
                    data = data + "DateBegin = " + logo.DateBegin + ";\n";
                    data = data + "DateEnd = " + logo.DateEnd + ";\n";
                    data = data + "Note = " + logo.Note + ";\n";
                    data = data + "IsSavedToPOS = " + logo.IsSavedToPOS + ";\n";
                    data = data + "IsSaved = " + logo.IsSaved + ";\n";
                    data = data + "DateS = " + logo.DateS + ";\n";
                    data = data + "DateE = " + logo.DateE + ";\n";
                    new Logs.Logs(currentUser, "EditLogo", data, "Дата начало должно быть меньше чем дата окончание действие логотипа").WriteErrorLogs();

                    #endregion

                    return View(logo);
                }
            }

            return View(logo);
        }

        #endregion

        #region Cashiers

        public async Task<ActionResult> Cashiers(string MarketID)
        {
            var currentUser = _userService.GetCurrentUser().Result;

            var cashiers = new Portal.DB.DB(_ctx, _sctx).GetCashiers(currentUser, MarketID);

            ViewBag.MarketID = new SelectList(cashiers.Markets != null ? cashiers.Markets : new List<MarketsName>(), "MarketID", "Name");

            #region Log

            new Logs.Logs(currentUser, "Cashiers", "", "").WriteInfoLogs();

            #endregion

            return View(cashiers);
        }

        // GET: Cashiers/CreateCashier
        public ActionResult CreateCashier()
        {
            var currentUser = _userService.GetCurrentUser().Result;

            Cashier cashier = new Cashier();

            var markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            #region Log

            new Logs.Logs(currentUser, "CreateCashier", "", "").WriteInfoLogs();

            #endregion

            return View(cashier);
        }

        // POST: Cashiers/CreateCashier
        [HttpPost]
        public async Task<ActionResult> CreateCashier(Cashier cashier, string[] SelectedMarkets)
        {
            var currentUser = _userService.GetCurrentUser().Result;

            List<MarketsName> markets = new List<MarketsName>();

            string _data = String.Empty;

            if (!string.IsNullOrEmpty(cashier.CashierName))
            {
                if (!string.IsNullOrEmpty(cashier.ID))
                {
                    if (SelectedMarkets.Length != 0)
                    {
                        if (cashier.ID.Length > 5 && cashier.ID.Length < 26)
                        {
                            if (IsDigitsOnly(cashier.ID))
                            {
                                for (int i = 0; i < SelectedMarkets.Length; i++)
                                {
                                    var checkCashierID = new Portal.DB.DB(_ctx, _sctx).GetCashier(cashier.ID, SelectedMarkets[i], currentUser);

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

                                            var isSaved = new Portal.DB.DB(_ctx, _sctx).SaveNewCashier(SelectedMarkets[i], _cashier, currentUser);

                                            #region Log

                                            _data = String.Empty;
                                            _data = "ID = " + _cashier.ID + ";\n";
                                            _data = _data + "TabelNumber = \"\";\n";
                                            _data = _data + "CashierName = " + _cashier.CashierName + ";\n";
                                            _data = _data + "Password = \"\";\n";
                                            _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                                            _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                                            _data = _data + "IsAdmin = " + _cashier.IsAdmin + ";\n";
                                            _data = _data + "IsDiscounter = " + _cashier.IsDiscounter + ";\n";
                                            _data = _data + "IsGoodDisco = false;\n";
                                            _data = _data + "IsInvoicer = false;\n";
                                            _data = _data + "IsSaved = false;\n";
                                            _data = _data + "IsSavedToPOS = 0;\n";
                                            _data = _data + "IsSavedToMarket = 0;\n";
                                            _data = _data + "MarketID = " + _cashier.MarketID + ";\n";

                                            new Logs.Logs(currentUser, "CreateCashier", _data, "Добавлен!").WriteInfoLogs();

                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                                        ViewBag.Markets = markets;
                                        ViewBag.MarketsCount = markets.Count;

                                        #region Log
                                        _data = String.Empty;
                                        _data = "ID = " + cashier.ID + ";\n";
                                        _data = _data + "TabelNumber = \"\";\n";
                                        _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                        _data = _data + "Password = \"\";\n";
                                        _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                                        _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                                        _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                        _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                        _data = _data + "IsGoodDisco = false;\n";
                                        _data = _data + "IsInvoicer = false;\n";
                                        _data = _data + "IsSaved = false;\n";
                                        _data = _data + "IsSavedToPOS = 0;\n";
                                        _data = _data + "IsSavedToMarket = 0;\n";
                                        _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                                        new Logs.Logs(currentUser, "CreateCashier", _data, "Кассир с таким именем или паролем уже существует!").WriteErrorLogs();

                                        #endregion

                                        TempData["msg"] = "<script>alert('Кассир с таким именем или паролем уже существует!');</script>";
                                        return View(cashier);
                                    }
                                }

                                return RedirectToAction("Cashiers");
                            }
                            else
                            {
                                markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                                ViewBag.Markets = markets;
                                ViewBag.MarketsCount = markets.Count;

                                #region Log
                                _data = String.Empty;
                                _data = "ID = " + cashier.ID + ";\n";
                                _data = _data + "TabelNumber = \"\";\n";
                                _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                _data = _data + "Password = \"\";\n";
                                _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                                _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                                _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                _data = _data + "IsGoodDisco = false;\n";
                                _data = _data + "IsInvoicer = false;\n";
                                _data = _data + "IsSaved = false;\n";
                                _data = _data + "IsSavedToPOS = 0;\n";
                                _data = _data + "IsSavedToMarket = 0;\n";
                                _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                                new Logs.Logs(currentUser, "CreateCashier", _data, "Кассир с таким именем или паролем уже существует!").WriteErrorLogs();

                                #endregion

                                TempData["msg"] = "<script>alert('Поле «Пароль» должен содержать только цифры!');</script>";
                                return View(cashier);
                            }
                        }
                        else
                        {
                            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                            ViewBag.Markets = markets;
                            ViewBag.MarketsCount = markets.Count;

                            #region Log
                            _data = String.Empty;
                            _data = "ID = " + cashier.ID + ";\n";
                            _data = _data + "TabelNumber = \"\";\n";
                            _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                            _data = _data + "Password = \"\";\n";
                            _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                            _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                            _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                            _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                            _data = _data + "IsGoodDisco = false;\n";
                            _data = _data + "IsInvoicer = false;\n";
                            _data = _data + "IsSaved = false;\n";
                            _data = _data + "IsSavedToPOS = 0;\n";
                            _data = _data + "IsSavedToMarket = 0;\n";
                            _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                            new Logs.Logs(currentUser, "CreateCashier", _data, "Длина пароля должна быть не меньше 6 и не больше 25 символов!").WriteErrorLogs();

                            #endregion

                            TempData["msg"] = "<script>alert('Длина пароля должна быть не меньше 6 и не больше 25 символов!');</script>";
                            return View(cashier);
                        }
                    }
                    else
                    {
                        if (cashier.ID.Length > 5 && cashier.ID.Length < 26)
                        {
                            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                            string mm = markets[0].MarketID;

                            var checkCashierID = new Portal.DB.DB(_ctx, _sctx).GetCashier(cashier.ID, mm, currentUser);

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

                                    var isSaved = new Portal.DB.DB(_ctx, _sctx).SaveNewCashier(mm, cashier, currentUser);

                                    #region Log
                                    _data = String.Empty;
                                    _data = "ID = " + cashier.ID + ";\n";
                                    _data = _data + "TabelNumber = \"\";\n";
                                    _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                    _data = _data + "Password = \"\";\n";
                                    _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                                    _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                                    _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                    _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                    _data = _data + "IsGoodDisco = false;\n";
                                    _data = _data + "IsInvoicer = false;\n";
                                    _data = _data + "IsSaved = false;\n";
                                    _data = _data + "IsSavedToPOS = 0;\n";
                                    _data = _data + "IsSavedToMarket = 0;\n";
                                    _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                                    new Logs.Logs(currentUser, "CreateCashier", _data, "Добавлен!").WriteInfoLogs();

                                    #endregion

                                    return RedirectToAction("Cashiers");
                                }
                                else
                                {
                                    ViewBag.Markets = markets;
                                    ViewBag.MarketsCount = markets.Count;

                                    #region Log
                                    _data = String.Empty;
                                    _data = "ID = " + cashier.ID + ";\n";
                                    _data = _data + "TabelNumber = \"\";\n";
                                    _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                    _data = _data + "Password = \"\";\n";
                                    _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                                    _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                                    _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                    _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                    _data = _data + "IsGoodDisco = false;\n";
                                    _data = _data + "IsInvoicer = false;\n";
                                    _data = _data + "IsSaved = false;\n";
                                    _data = _data + "IsSavedToPOS = 0;\n";
                                    _data = _data + "IsSavedToMarket = 0;\n";
                                    _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                                    new Logs.Logs(currentUser, "CreateCashier", _data, "Поле «Пароль» должен содержать только цифры!").WriteErrorLogs();

                                    #endregion

                                    TempData["msg"] = "<script>alert('Поле «Пароль» должен содержать только цифры!');</script>";
                                    return View(cashier);
                                }
                            }
                            else
                            {
                                markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                                ViewBag.Markets = markets;
                                ViewBag.MarketsCount = markets.Count;

                                #region Log
                                _data = String.Empty;
                                _data = "ID = " + cashier.ID + ";\n";
                                _data = _data + "TabelNumber = \"\";\n";
                                _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                                _data = _data + "Password = \"\";\n";
                                _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                                _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                                _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                                _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                                _data = _data + "IsGoodDisco = false;\n";
                                _data = _data + "IsInvoicer = false;\n";
                                _data = _data + "IsSaved = false;\n";
                                _data = _data + "IsSavedToPOS = 0;\n";
                                _data = _data + "IsSavedToMarket = 0;\n";
                                _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                                new Logs.Logs(currentUser, "CreateCashier", _data, "Пользователь с таким номером уже существует!").WriteErrorLogs();

                                #endregion

                                TempData["msg"] = "<script>alert('Пользователь с таким номером уже существует!');</script>";
                                return View(cashier);
                            }
                        }
                        else
                        {
                            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                            ViewBag.Markets = markets;
                            ViewBag.MarketsCount = markets.Count;

                            #region Log
                            _data = String.Empty;
                            _data = "ID = " + cashier.ID + ";\n";
                            _data = _data + "TabelNumber = \"\";\n";
                            _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                            _data = _data + "Password = \"\";\n";
                            _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                            _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                            _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                            _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                            _data = _data + "IsGoodDisco = false;\n";
                            _data = _data + "IsInvoicer = false;\n";
                            _data = _data + "IsSaved = false;\n";
                            _data = _data + "IsSavedToPOS = 0;\n";
                            _data = _data + "IsSavedToMarket = 0;\n";
                            _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                            new Logs.Logs(currentUser, "CreateCashier", _data, "Длина пароля должна быть не меньше 6 и не больше 25 символов!").WriteErrorLogs();

                            #endregion

                            TempData["msg"] = "<script>alert('Длина пароля должна быть не меньше 6 и не больше 25 символов!');</script>";
                            return View(cashier);
                        }
                    }
                }

                markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                ViewBag.Markets = markets;
                ViewBag.MarketsCount = markets.Count;

                #region Log
                _data = String.Empty;
                _data = "ID = " + cashier.ID + ";\n";
                _data = _data + "TabelNumber = \"\";\n";
                _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                _data = _data + "Password = \"\";\n";
                _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                _data = _data + "IsGoodDisco = false;\n";
                _data = _data + "IsInvoicer = false;\n";
                _data = _data + "IsSaved = false;\n";
                _data = _data + "IsSavedToPOS = 0;\n";
                _data = _data + "IsSavedToMarket = 0;\n";
                _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                new Logs.Logs(currentUser, "CreateCashier", _data, "Некорректное заполнение полей!").WriteErrorLogs();

                #endregion

                TempData["msg"] = "<script>alert('Некорректное заполнение полей!');</script>";
                return View(cashier);
            }

            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            #region Log
            _data = String.Empty;
            _data = "ID = " + cashier.ID + ";\n";
            _data = _data + "TabelNumber = \"\";\n";
            _data = _data + "CashierName = " + cashier.CashierName + ";\n";
            _data = _data + "Password = \"\";\n";
            _data = _data + "DateBegin = " + DateTime.Now + ";\n";
            _data = _data + "DateEnd = " + DateTime.Now + ";\n";
            _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
            _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
            _data = _data + "IsGoodDisco = false;\n";
            _data = _data + "IsInvoicer = false;\n";
            _data = _data + "IsSaved = false;\n";
            _data = _data + "IsSavedToPOS = 0;\n";
            _data = _data + "IsSavedToMarket = 0;\n";
            _data = _data + "MarketID = " + cashier.MarketID + ";\n";

            new Logs.Logs(currentUser, "CreateCashier", _data, "Некорректное заполнение полей!").WriteErrorLogs();

            #endregion

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

        public async Task<ActionResult> EditCashier(string id, string cashierName, string marketID)
        {
            var currentUser = _userService.GetCurrentUser();

            Cashier cashier = new Portal.DB.DB(_ctx, _sctx).GetCashier(id, marketID, currentUser.Result);

            if (cashier == null)
                return NotFound();

            var markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser.Result);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            #region Log

            new Logs.Logs(currentUser.Result, "EditCashier", "", "").WriteInfoLogs();

            #endregion

            return View(cashier);
        }

        // POST: Cashiers/EditCashier
        [HttpPost]
        public async Task<ActionResult> EditCashier(Cashier cashier)
        {
            List<MarketsName> markets;
            var currentUser = _userService.GetCurrentUser().Result;
            string _data = String.Empty;

            if (!string.IsNullOrEmpty(cashier.CashierName))
            {
                if (!string.IsNullOrEmpty(cashier.ID))
                {
                    if (cashier.ID.Length > 5 && cashier.ID.Length < 26)
                    {
                        try
                        {
                            new Portal.DB.DB(_ctx, _sctx).EditCashier(cashier, currentUser);

                            #region Log
                            _data = String.Empty;
                            _data = "ID = " + cashier.ID + ";\n";
                            _data = _data + "TabelNumber = \"\";\n";
                            _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                            _data = _data + "Password = \"\";\n";
                            _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                            _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                            _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                            _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                            _data = _data + "IsGoodDisco = false;\n";
                            _data = _data + "IsInvoicer = false;\n";
                            _data = _data + "IsSaved = false;\n";
                            _data = _data + "IsSavedToPOS = 0;\n";
                            _data = _data + "IsSavedToMarket = 0;\n";
                            _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                            new Logs.Logs(currentUser, "EditCashier", _data, "Изменен!").WriteInfoLogs();

                            #endregion

                            return RedirectToAction("Cashiers");
                        }
                        catch (Exception)
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                        ViewBag.Markets = markets;
                        ViewBag.MarketsCount = markets.Count;

                        #region Log
                        _data = String.Empty;
                        _data = "ID = " + cashier.ID + ";\n";
                        _data = _data + "TabelNumber = \"\";\n";
                        _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                        _data = _data + "Password = \"\";\n";
                        _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                        _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                        _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                        _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                        _data = _data + "IsGoodDisco = false;\n";
                        _data = _data + "IsInvoicer = false;\n";
                        _data = _data + "IsSaved = false;\n";
                        _data = _data + "IsSavedToPOS = 0;\n";
                        _data = _data + "IsSavedToMarket = 0;\n";
                        _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                        new Logs.Logs(currentUser, "EditCashier", _data, "Длина пароля должно быть не меньше 6 и не больше 25 символов!").WriteErrorLogs();

                        #endregion

                        TempData["msg"] = "<script>alert('Длина пароля должно быть не меньше 6 и не больше 25 символов!');</script>";
                        return View(cashier);
                    }
                }

                markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
                ViewBag.Markets = markets;
                ViewBag.MarketsCount = markets.Count;

                #region Log
                _data = String.Empty;
                _data = "ID = " + cashier.ID + ";\n";
                _data = _data + "TabelNumber = \"\";\n";
                _data = _data + "CashierName = " + cashier.CashierName + ";\n";
                _data = _data + "Password = \"\";\n";
                _data = _data + "DateBegin = " + DateTime.Now + ";\n";
                _data = _data + "DateEnd = " + DateTime.Now + ";\n";
                _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
                _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
                _data = _data + "IsGoodDisco = false;\n";
                _data = _data + "IsInvoicer = false;\n";
                _data = _data + "IsSaved = false;\n";
                _data = _data + "IsSavedToPOS = 0;\n";
                _data = _data + "IsSavedToMarket = 0;\n";
                _data = _data + "MarketID = " + cashier.MarketID + ";\n";

                new Logs.Logs(currentUser, "EditCashier", _data, "Некорректное заполнение полей!").WriteErrorLogs();

                #endregion

                TempData["msg"] = "<script>alert('Некорректное заполнение полей!');</script>";
                return View(cashier);
            }

            markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;

            #region Log
            _data = String.Empty;
            _data = "ID = " + cashier.ID + ";\n";
            _data = _data + "TabelNumber = \"\";\n";
            _data = _data + "CashierName = " + cashier.CashierName + ";\n";
            _data = _data + "Password = \"\";\n";
            _data = _data + "DateBegin = " + DateTime.Now + ";\n";
            _data = _data + "DateEnd = " + DateTime.Now + ";\n";
            _data = _data + "IsAdmin = " + cashier.IsAdmin + ";\n";
            _data = _data + "IsDiscounter = " + cashier.IsDiscounter + ";\n";
            _data = _data + "IsGoodDisco = false;\n";
            _data = _data + "IsInvoicer = false;\n";
            _data = _data + "IsSaved = false;\n";
            _data = _data + "IsSavedToPOS = 0;\n";
            _data = _data + "IsSavedToMarket = 0;\n";
            _data = _data + "MarketID = " + cashier.MarketID + ";\n";

            new Logs.Logs(currentUser, "EditCashier", _data, "Некорректное заполнение полей!").WriteErrorLogs();

            #endregion

            TempData["msg"] = "<script>alert('Некорректное заполнение полей!');</script>";
            return View(cashier);
        }

        [HttpGet]
        [ActionName("DeleteCashier")]
        public async Task<IActionResult> ConfirmDeleteCashier(int? id)
        {
            if (id != null)
            {
                var currentUser = _userService.GetCurrentUser().Result;
                User user = new Portal.DB.DB(_ctx, _sctx).GetUser(id, currentUser);

                ViewBag.CurrentMarket = new Portal.DB.DB(_ctx, _sctx).GetMarkets(user.MarketID, currentUser).Name;

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
                var currentUser = _userService.GetCurrentUser().Result;
                var isDeleted = new Portal.DB.DB(_ctx, _sctx).DeleteUser(id, currentUser);

                #region Log
                new Logs.Logs(currentUser, "EditCashier", "", "Удален!").WriteInfoLogs();
                #endregion

                return RedirectToAction("Cashiers");
            }
            return NotFound();
        }

        #endregion

        #region Keyboards

        public ActionResult Keyboards(string MarketID)
        {
            var currentUser = _userService.GetCurrentUser().Result;

            var keyboards = new Portal.DB.DB(_ctx, _sctx).GetKeyboards(currentUser, MarketID);

            if (string.IsNullOrEmpty(MarketID))
                ViewBag.SelectText = false;
            else
                ViewBag.SelectText = true;

            ViewBag.MarketID = new SelectList(keyboards.Markets != null ? keyboards.Markets : new List<MarketsName>(), "MarketID", "Name");

            TempData["CurrentMarketID"] = MarketID;

            #region Log

            new Logs.Logs(currentUser, "Keyboards", "", "").WriteInfoLogs();

            #endregion

            return View(keyboards);
        }

        // GET: Keyboards/CreateKeyboard
        public ActionResult CreateKeyboard()
        {
            var currentUser = _userService.GetCurrentUser().Result;

            Keyboard keyboard = new Keyboard();

            ViewBag.MarketID = TempData.Peek("CurrentMarketID");

            ViewBag.SettingsKey = new Portal.DB.DB(_ctx, _sctx).GetKeys(currentUser);

            #region Log

            new Logs.Logs(currentUser, "CreateKeyboard", "", "").WriteInfoLogs();

            #endregion

            return View(keyboard);
        }

        // POST: Keyboards/CreateKeyboard
        [HttpPost]
        public async Task<ActionResult> CreateKeyboard(Keyboard keyboard, string[] btnkeyH, string MarketForUser)
        {
            string _data = string.Empty;
            var currentUser = _userService.GetCurrentUser().Result;

            List<string> valLst = new List<string>();

            var settingsKey = new Portal.DB.DB(_ctx, _sctx).GetKeys(currentUser);

            for (int i = 0; i < btnkeyH.Length; i++)
            {
                string value = btnkeyH[i].ToString().Split(':')[1].Trim().Replace("\n", "");

                if (value.Length > 2)
                {
                    var keyCode = settingsKey.Where(w => w.Value == value).ToList();

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

            if (valLst.Count > 0)
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

                if (!currentUser.IsAdmin && !currentUser.Role.AllMarkets)
                    keyboard.MarketID = currentUser.MarketID;
                else
                    keyboard.MarketID = MarketForUser;

                keyboard.Pos_num = 0;
                keyboard.IsSavedToPOS = 0;
                keyboard.IsSaved = false;

                var isSaved = new Portal.DB.DB(_ctx, _sctx).SaveNewKeyboard(keyboard, currentUser);

                #region Log

                _data = "ID = " + keyboard.ID + ";\n";
                _data = _data + "MarketID = " + keyboard.MarketID + ";\n";
                _data = _data + "Pos_num  = " + keyboard.Pos_num + ";\n";
                _data = _data + "Key_1 = " + keyboard.Key_1 + ";\n";
                _data = _data + "Key_2 = " + keyboard.Key_2 + ";\n";
                _data = _data + "Key_3 = " + keyboard.Key_3 + ";\n";
                _data = _data + "Key_4 = " + keyboard.Key_4 + ";\n";
                _data = _data + "Key_5 = " + keyboard.Key_5 + ";\n";
                _data = _data + "Key_6 = " + keyboard.Key_6 + ";\n";
                _data = _data + "Key_7 = " + keyboard.Key_7 + ";\n";
                _data = _data + "Key_8 = " + keyboard.Key_8 + ";\n";
                _data = _data + "Key_9 = " + keyboard.Key_9 + ";\n";
                _data = _data + "Key_10 = " + keyboard.Key_10 + ";\n";
                _data = _data + "Key_11 = " + keyboard.Key_11 + ";\n";
                _data = _data + "Key_12 = " + keyboard.Key_12 + ";\n";
                _data = _data + "Key_13 = " + keyboard.Key_13 + ";\n";
                _data = _data + "Key_14 = " + keyboard.Key_14 + ";\n";
                _data = _data + "Key_15 = " + keyboard.Key_15 + ";\n";
                _data = _data + "Key_16 = " + keyboard.Key_16 + ";\n";
                _data = _data + "Key_17 = " + keyboard.Key_17 + ";\n";
                _data = _data + "Key_30 = " + keyboard.Key_30 + ";\n";
                _data = _data + "Key_31 = " + keyboard.Key_31 + ";\n";
                _data = _data + "Key_32 = " + keyboard.Key_32 + ";\n";
                _data = _data + "Key_33 = " + keyboard.Key_33 + ";\n";
                _data = _data + "Key_34 = " + keyboard.Key_34 + ";\n";
                _data = _data + "Key_35 = " + keyboard.Key_35 + ";\n";
                _data = _data + "Key_36 = " + keyboard.Key_36 + ";\n";
                _data = _data + "Key_37 = " + keyboard.Key_37 + ";\n";
                _data = _data + "Key_38 = " + keyboard.Key_38 + ";\n";
                _data = _data + "Key_39 = " + keyboard.Key_39 + ";\n";
                _data = _data + "Key_40 = " + keyboard.Key_40 + ";\n";
                _data = _data + "Key_41 = " + keyboard.Key_41 + ";\n";
                _data = _data + "Key_42 = " + keyboard.Key_42 + ";\n";
                _data = _data + "Key_43 = " + keyboard.Key_43 + ";\n";
                _data = _data + "Key_44 = " + keyboard.Key_44 + ";\n";
                _data = _data + "Key_45 = " + keyboard.Key_45 + ";\n";
                _data = _data + "Key_46 = " + keyboard.Key_46 + ";\n";
                _data = _data + "Key_47 = " + keyboard.Key_47 + ";\n";
                _data = _data + "Key_48 = " + keyboard.Key_48 + ";\n";
                _data = _data + "Key_49 = " + keyboard.Key_49 + ";\n";
                _data = _data + "Key_50 = " + keyboard.Key_50 + ";\n";
                _data = _data + "Key_51 = " + keyboard.Key_51 + ";\n";
                _data = _data + "Key_52 = " + keyboard.Key_52 + ";\n";
                _data = _data + "Key_53 = " + keyboard.Key_53 + ";\n";
                _data = _data + "Key_54 = " + keyboard.Key_54 + ";\n";
                _data = _data + "Key_55 = " + keyboard.Key_55 + ";\n";
                _data = _data + "Key_56 = " + keyboard.Key_56 + ";\n";
                _data = _data + "Key_57 = " + keyboard.Key_57 + ";\n";
                _data = _data + "Key_58 = " + keyboard.Key_58 + ";\n";
                _data = _data + "Key_59 = " + keyboard.Key_59 + ";\n";
                _data = _data + "Key_60 = " + keyboard.Key_60 + ";\n";
                _data = _data + "Key_61 = " + keyboard.Key_61 + ";\n";
                _data = _data + "Key_62 = " + keyboard.Key_62 + ";\n";
                _data = _data + "Key_63 = " + keyboard.Key_63 + ";\n";
                _data = _data + "Key_64 = " + keyboard.Key_64 + ";\n";
                _data = _data + "Key_65 = " + keyboard.Key_65 + ";\n";
                _data = _data + "Key_66 = " + keyboard.Key_66 + ";\n";
                _data = _data + "Key_67 = " + keyboard.Key_67 + ";\n";
                new Logs.Logs(currentUser, "CreateKeyboard", _data, "Добавлен!").WriteInfoLogs();

                #endregion

                return RedirectToAction("Keyboards");
            }

            var markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;
            ViewBag.SettingsKey = settingsKey;

            TempData["msg"] = "<script>alert('Не корректное заполнение полей!');</script>";

            #region Log

            _data = "ID = " + keyboard.ID + ";\n";
            _data = _data + "MarketID = " + keyboard.MarketID + ";\n";
            _data = _data + "Pos_num  = " + keyboard.Pos_num + ";\n";
            _data = _data + "Key_1 = " + keyboard.Key_1 + ";\n";
            _data = _data + "Key_2 = " + keyboard.Key_2 + ";\n";
            _data = _data + "Key_3 = " + keyboard.Key_3 + ";\n";
            _data = _data + "Key_4 = " + keyboard.Key_4 + ";\n";
            _data = _data + "Key_5 = " + keyboard.Key_5 + ";\n";
            _data = _data + "Key_6 = " + keyboard.Key_6 + ";\n";
            _data = _data + "Key_7 = " + keyboard.Key_7 + ";\n";
            _data = _data + "Key_8 = " + keyboard.Key_8 + ";\n";
            _data = _data + "Key_9 = " + keyboard.Key_9 + ";\n";
            _data = _data + "Key_10 = " + keyboard.Key_10 + ";\n";
            _data = _data + "Key_11 = " + keyboard.Key_11 + ";\n";
            _data = _data + "Key_12 = " + keyboard.Key_12 + ";\n";
            _data = _data + "Key_13 = " + keyboard.Key_13 + ";\n";
            _data = _data + "Key_14 = " + keyboard.Key_14 + ";\n";
            _data = _data + "Key_15 = " + keyboard.Key_15 + ";\n";
            _data = _data + "Key_16 = " + keyboard.Key_16 + ";\n";
            _data = _data + "Key_17 = " + keyboard.Key_17 + ";\n";
            _data = _data + "Key_30 = " + keyboard.Key_30 + ";\n";
            _data = _data + "Key_31 = " + keyboard.Key_31 + ";\n";
            _data = _data + "Key_32 = " + keyboard.Key_32 + ";\n";
            _data = _data + "Key_33 = " + keyboard.Key_33 + ";\n";
            _data = _data + "Key_34 = " + keyboard.Key_34 + ";\n";
            _data = _data + "Key_35 = " + keyboard.Key_35 + ";\n";
            _data = _data + "Key_36 = " + keyboard.Key_36 + ";\n";
            _data = _data + "Key_37 = " + keyboard.Key_37 + ";\n";
            _data = _data + "Key_38 = " + keyboard.Key_38 + ";\n";
            _data = _data + "Key_39 = " + keyboard.Key_39 + ";\n";
            _data = _data + "Key_40 = " + keyboard.Key_40 + ";\n";
            _data = _data + "Key_41 = " + keyboard.Key_41 + ";\n";
            _data = _data + "Key_42 = " + keyboard.Key_42 + ";\n";
            _data = _data + "Key_43 = " + keyboard.Key_43 + ";\n";
            _data = _data + "Key_44 = " + keyboard.Key_44 + ";\n";
            _data = _data + "Key_45 = " + keyboard.Key_45 + ";\n";
            _data = _data + "Key_46 = " + keyboard.Key_46 + ";\n";
            _data = _data + "Key_47 = " + keyboard.Key_47 + ";\n";
            _data = _data + "Key_48 = " + keyboard.Key_48 + ";\n";
            _data = _data + "Key_49 = " + keyboard.Key_49 + ";\n";
            _data = _data + "Key_50 = " + keyboard.Key_50 + ";\n";
            _data = _data + "Key_51 = " + keyboard.Key_51 + ";\n";
            _data = _data + "Key_52 = " + keyboard.Key_52 + ";\n";
            _data = _data + "Key_53 = " + keyboard.Key_53 + ";\n";
            _data = _data + "Key_54 = " + keyboard.Key_54 + ";\n";
            _data = _data + "Key_55 = " + keyboard.Key_55 + ";\n";
            _data = _data + "Key_56 = " + keyboard.Key_56 + ";\n";
            _data = _data + "Key_57 = " + keyboard.Key_57 + ";\n";
            _data = _data + "Key_58 = " + keyboard.Key_58 + ";\n";
            _data = _data + "Key_59 = " + keyboard.Key_59 + ";\n";
            _data = _data + "Key_60 = " + keyboard.Key_60 + ";\n";
            _data = _data + "Key_61 = " + keyboard.Key_61 + ";\n";
            _data = _data + "Key_62 = " + keyboard.Key_62 + ";\n";
            _data = _data + "Key_63 = " + keyboard.Key_63 + ";\n";
            _data = _data + "Key_64 = " + keyboard.Key_64 + ";\n";
            _data = _data + "Key_65 = " + keyboard.Key_65 + ";\n";
            _data = _data + "Key_66 = " + keyboard.Key_66 + ";\n";
            _data = _data + "Key_67 = " + keyboard.Key_67 + ";\n";
            new Logs.Logs(currentUser, "CreateKeyboard", _data, "Не корректное заполнение полей!").WriteErrorLogs();

            #endregion

            return View(keyboard);
        }

        // GET: Keyboards/EditKeyboard
        public ActionResult EditKeyboard(int? id)
        {
            if (id == null)
                return NotFound();

            var currentUser = _userService.GetCurrentUser().Result;

            var sk = new Portal.DB.DB(_ctx, _sctx).GetKeys(currentUser);
            Keyboard keyboard = new Portal.DB.DB(_ctx, _sctx).GetKeyboard(id, currentUser);
            keyboard = ReplaceKeysValue(keyboard, sk);

            if (keyboard == null)
                return NotFound();

            ViewBag.MarketID = keyboard.MarketID;
            ViewBag.SettingsKey = sk;

            #region Log
            
            new Logs.Logs(currentUser, "EditKeyboard", "", "").WriteInfoLogs();

            #endregion

            return View(keyboard);
        }

        // POST: Keyboards/EditKeyboard
        [HttpPost]
        public async Task<ActionResult> EditKeyboard(Keyboard keyboard, string[] btnkeyH, string MarketForUser)
        {
            string _data = string.Empty;

            var currentUser = _userService.GetCurrentUser().Result;

            List<string> valLst = new List<string>();

            var settingsKey = new Portal.DB.DB(_ctx, _sctx).GetKeys(currentUser);

            for (int i = 0; i < btnkeyH.Length; i++)
            {
                string value = btnkeyH[i].ToString().Split(':')[1].Trim().Replace("\n", "");

                if (value.Length > 2)
                {
                    var keyCode = settingsKey.Where(w => w.Value == value).ToList();

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

            if (!string.IsNullOrEmpty(MarketForUser) && valLst.Count > 0)
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

                var isSaved = new Portal.DB.DB(_ctx, _sctx).EditKeyboard(keyboard, currentUser);

                #region Log

                _data = "ID = " + keyboard.ID + ";\n";
                _data = _data + "MarketID = " + keyboard.MarketID + ";\n";
                _data = _data + "Pos_num  = " + keyboard.Pos_num + ";\n";
                _data = _data + "Key_1 = " + keyboard.Key_1 + ";\n";
                _data = _data + "Key_2 = " + keyboard.Key_2 + ";\n";
                _data = _data + "Key_3 = " + keyboard.Key_3 + ";\n";
                _data = _data + "Key_4 = " + keyboard.Key_4 + ";\n";
                _data = _data + "Key_5 = " + keyboard.Key_5 + ";\n";
                _data = _data + "Key_6 = " + keyboard.Key_6 + ";\n";
                _data = _data + "Key_7 = " + keyboard.Key_7 + ";\n";
                _data = _data + "Key_8 = " + keyboard.Key_8 + ";\n";
                _data = _data + "Key_9 = " + keyboard.Key_9 + ";\n";
                _data = _data + "Key_10 = " + keyboard.Key_10 + ";\n";
                _data = _data + "Key_11 = " + keyboard.Key_11 + ";\n";
                _data = _data + "Key_12 = " + keyboard.Key_12 + ";\n";
                _data = _data + "Key_13 = " + keyboard.Key_13 + ";\n";
                _data = _data + "Key_14 = " + keyboard.Key_14 + ";\n";
                _data = _data + "Key_15 = " + keyboard.Key_15 + ";\n";
                _data = _data + "Key_16 = " + keyboard.Key_16 + ";\n";
                _data = _data + "Key_17 = " + keyboard.Key_17 + ";\n";
                _data = _data + "Key_30 = " + keyboard.Key_30 + ";\n";
                _data = _data + "Key_31 = " + keyboard.Key_31 + ";\n";
                _data = _data + "Key_32 = " + keyboard.Key_32 + ";\n";
                _data = _data + "Key_33 = " + keyboard.Key_33 + ";\n";
                _data = _data + "Key_34 = " + keyboard.Key_34 + ";\n";
                _data = _data + "Key_35 = " + keyboard.Key_35 + ";\n";
                _data = _data + "Key_36 = " + keyboard.Key_36 + ";\n";
                _data = _data + "Key_37 = " + keyboard.Key_37 + ";\n";
                _data = _data + "Key_38 = " + keyboard.Key_38 + ";\n";
                _data = _data + "Key_39 = " + keyboard.Key_39 + ";\n";
                _data = _data + "Key_40 = " + keyboard.Key_40 + ";\n";
                _data = _data + "Key_41 = " + keyboard.Key_41 + ";\n";
                _data = _data + "Key_42 = " + keyboard.Key_42 + ";\n";
                _data = _data + "Key_43 = " + keyboard.Key_43 + ";\n";
                _data = _data + "Key_44 = " + keyboard.Key_44 + ";\n";
                _data = _data + "Key_45 = " + keyboard.Key_45 + ";\n";
                _data = _data + "Key_46 = " + keyboard.Key_46 + ";\n";
                _data = _data + "Key_47 = " + keyboard.Key_47 + ";\n";
                _data = _data + "Key_48 = " + keyboard.Key_48 + ";\n";
                _data = _data + "Key_49 = " + keyboard.Key_49 + ";\n";
                _data = _data + "Key_50 = " + keyboard.Key_50 + ";\n";
                _data = _data + "Key_51 = " + keyboard.Key_51 + ";\n";
                _data = _data + "Key_52 = " + keyboard.Key_52 + ";\n";
                _data = _data + "Key_53 = " + keyboard.Key_53 + ";\n";
                _data = _data + "Key_54 = " + keyboard.Key_54 + ";\n";
                _data = _data + "Key_55 = " + keyboard.Key_55 + ";\n";
                _data = _data + "Key_56 = " + keyboard.Key_56 + ";\n";
                _data = _data + "Key_57 = " + keyboard.Key_57 + ";\n";
                _data = _data + "Key_58 = " + keyboard.Key_58 + ";\n";
                _data = _data + "Key_59 = " + keyboard.Key_59 + ";\n";
                _data = _data + "Key_60 = " + keyboard.Key_60 + ";\n";
                _data = _data + "Key_61 = " + keyboard.Key_61 + ";\n";
                _data = _data + "Key_62 = " + keyboard.Key_62 + ";\n";
                _data = _data + "Key_63 = " + keyboard.Key_63 + ";\n";
                _data = _data + "Key_64 = " + keyboard.Key_64 + ";\n";
                _data = _data + "Key_65 = " + keyboard.Key_65 + ";\n";
                _data = _data + "Key_66 = " + keyboard.Key_66 + ";\n";
                _data = _data + "Key_67 = " + keyboard.Key_67 + ";\n";
                new Logs.Logs(currentUser, "EditKeyboard", _data, "Изменен!").WriteInfoLogs();

                #endregion

                return RedirectToAction("Keyboards");
            }

            var markets = new Portal.DB.DB(_ctx, _sctx).GetMarketsForPrivileges(currentUser);
            ViewBag.Markets = markets;
            ViewBag.MarketsCount = markets.Count;
            ViewBag.SettingsKey = settingsKey;

            TempData["msg"] = "<script>alert('Не корректное заполнение полей!');</script>";

            #region Log

            _data = "ID = " + keyboard.ID + ";\n";
            _data = _data + "MarketID = " + keyboard.MarketID + ";\n";
            _data = _data + "Pos_num  = " + keyboard.Pos_num + ";\n";
            _data = _data + "Key_1 = " + keyboard.Key_1 + ";\n";
            _data = _data + "Key_2 = " + keyboard.Key_2 + ";\n";
            _data = _data + "Key_3 = " + keyboard.Key_3 + ";\n";
            _data = _data + "Key_4 = " + keyboard.Key_4 + ";\n";
            _data = _data + "Key_5 = " + keyboard.Key_5 + ";\n";
            _data = _data + "Key_6 = " + keyboard.Key_6 + ";\n";
            _data = _data + "Key_7 = " + keyboard.Key_7 + ";\n";
            _data = _data + "Key_8 = " + keyboard.Key_8 + ";\n";
            _data = _data + "Key_9 = " + keyboard.Key_9 + ";\n";
            _data = _data + "Key_10 = " + keyboard.Key_10 + ";\n";
            _data = _data + "Key_11 = " + keyboard.Key_11 + ";\n";
            _data = _data + "Key_12 = " + keyboard.Key_12 + ";\n";
            _data = _data + "Key_13 = " + keyboard.Key_13 + ";\n";
            _data = _data + "Key_14 = " + keyboard.Key_14 + ";\n";
            _data = _data + "Key_15 = " + keyboard.Key_15 + ";\n";
            _data = _data + "Key_16 = " + keyboard.Key_16 + ";\n";
            _data = _data + "Key_17 = " + keyboard.Key_17 + ";\n";
            _data = _data + "Key_30 = " + keyboard.Key_30 + ";\n";
            _data = _data + "Key_31 = " + keyboard.Key_31 + ";\n";
            _data = _data + "Key_32 = " + keyboard.Key_32 + ";\n";
            _data = _data + "Key_33 = " + keyboard.Key_33 + ";\n";
            _data = _data + "Key_34 = " + keyboard.Key_34 + ";\n";
            _data = _data + "Key_35 = " + keyboard.Key_35 + ";\n";
            _data = _data + "Key_36 = " + keyboard.Key_36 + ";\n";
            _data = _data + "Key_37 = " + keyboard.Key_37 + ";\n";
            _data = _data + "Key_38 = " + keyboard.Key_38 + ";\n";
            _data = _data + "Key_39 = " + keyboard.Key_39 + ";\n";
            _data = _data + "Key_40 = " + keyboard.Key_40 + ";\n";
            _data = _data + "Key_41 = " + keyboard.Key_41 + ";\n";
            _data = _data + "Key_42 = " + keyboard.Key_42 + ";\n";
            _data = _data + "Key_43 = " + keyboard.Key_43 + ";\n";
            _data = _data + "Key_44 = " + keyboard.Key_44 + ";\n";
            _data = _data + "Key_45 = " + keyboard.Key_45 + ";\n";
            _data = _data + "Key_46 = " + keyboard.Key_46 + ";\n";
            _data = _data + "Key_47 = " + keyboard.Key_47 + ";\n";
            _data = _data + "Key_48 = " + keyboard.Key_48 + ";\n";
            _data = _data + "Key_49 = " + keyboard.Key_49 + ";\n";
            _data = _data + "Key_50 = " + keyboard.Key_50 + ";\n";
            _data = _data + "Key_51 = " + keyboard.Key_51 + ";\n";
            _data = _data + "Key_52 = " + keyboard.Key_52 + ";\n";
            _data = _data + "Key_53 = " + keyboard.Key_53 + ";\n";
            _data = _data + "Key_54 = " + keyboard.Key_54 + ";\n";
            _data = _data + "Key_55 = " + keyboard.Key_55 + ";\n";
            _data = _data + "Key_56 = " + keyboard.Key_56 + ";\n";
            _data = _data + "Key_57 = " + keyboard.Key_57 + ";\n";
            _data = _data + "Key_58 = " + keyboard.Key_58 + ";\n";
            _data = _data + "Key_59 = " + keyboard.Key_59 + ";\n";
            _data = _data + "Key_60 = " + keyboard.Key_60 + ";\n";
            _data = _data + "Key_61 = " + keyboard.Key_61 + ";\n";
            _data = _data + "Key_62 = " + keyboard.Key_62 + ";\n";
            _data = _data + "Key_63 = " + keyboard.Key_63 + ";\n";
            _data = _data + "Key_64 = " + keyboard.Key_64 + ";\n";
            _data = _data + "Key_65 = " + keyboard.Key_65 + ";\n";
            _data = _data + "Key_66 = " + keyboard.Key_66 + ";\n";
            _data = _data + "Key_67 = " + keyboard.Key_67 + ";\n";
            new Logs.Logs(currentUser, "EditKeyboard", _data, "Не корректное заполнение полей!").WriteErrorLogs();

            #endregion

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

        #region POSes

        public ActionResult POSs(string MarketID)
        {
            var currentUser = _userService.GetCurrentUser().Result;

            var _userPOSs = new Portal.DB.DB(_ctx, _sctx).GetUserForPOS(currentUser, MarketID);
            var pos = new Portal.DB.DB(_ctx, _sctx).GetPOSes(_userPOSs, currentUser);
            ViewBag.ErrorPOSCount = pos.Items.Where(w => w.Status == "Не загружено").ToList().Count;

            #region Log

            new Logs.Logs(currentUser, "POSs", "", "Просмотр POS статуса!").WriteInfoLogs();

            #endregion

            return View(pos);
        }

        #endregion

        #region Scales

        public ActionResult Scales(string MarketID)
        {
            var currentUser = _userService.GetCurrentUser().Result;
            var _userScales = new Portal.DB.DB(_ctx, _sctx).GetUserForScales(currentUser, MarketID);
            var scales = new Portal.DB.DB(_ctx, _sctx).GetScales(_userScales);

            #region Log

            new Logs.Logs(currentUser, "Scales", "", "Просмотр статуса весов!").WriteInfoLogs();

            #endregion

            return View(scales);
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