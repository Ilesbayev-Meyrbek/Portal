using Microsoft.AspNetCore.Mvc;
using Portal.Services.Interfaces;
using UZ.STS.POS2K.DataAccess;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsAPIController : ControllerBase
    {
        private readonly DataContext _ctx;

        private readonly IMarketService _marketService;
        private readonly IChequeService _chequeService;

        public ReportsAPIController(DataContext ctx, IMarketService marketService, IChequeService chequeService)
        {
            this._ctx = ctx;
            _marketService = marketService;
            _chequeService = chequeService;
        }

        [Route("GetMarketsByPOS")]
        [HttpGet]
        public async Task<List<MarketsName>> GetMarketsByPOS()
        {
            var markets = await _marketService.GetAllAsync();

            return markets.Data;
        }

        //[Route("GetTerminals")]
        //[HttpGet]
        //public async Task<List<string>> GetTerminals()
        //{
        //    var terminalIDs = await _chequeService.GetAllTerminalIDAsync();

        //    return terminalIDs.Data;
        //}
    }
}
