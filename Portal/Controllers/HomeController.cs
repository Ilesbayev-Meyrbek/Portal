using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.DB;
using Portal.Models;
using System.Diagnostics;

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