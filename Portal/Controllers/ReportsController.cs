using Microsoft.AspNetCore.Mvc;
using Portal.DTO;
using Portal.Services;
using Portal.Services.Interfaces;

namespace Portal.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IChequeService _chequeService;
        private readonly IChequeGoodService _chequeGoodService;
        private readonly IChequeGoodDiscountService _chequeGoodDiscountService;

        public ReportsController(IChequeService chequeService, IChequeGoodService chequeGoodService, IChequeGoodDiscountService chequeGoodDiscountService)
        {
            _chequeService = chequeService;
            _chequeGoodService = chequeGoodService;
            _chequeGoodDiscountService = chequeGoodDiscountService;
        }

        public ActionResult Reports()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Reports(
            DateTime DateBegin,
            DateTime DateEnd,
            string MarketsCheckboxName,
            string selectMarketID,
            int selectPos,
            string TerminalsCheckboxName,
            string terminalID)
        {
            int dateBegin = int.Parse(DateBegin.ToString("yyyyMMdd"));
            int dateEnd = int.Parse(DateEnd.ToString("yyyyMMdd"));

            var days = DateEnd - DateBegin;

            if (days.Days <= 31)
            {
                bool checkBoxMarkets = false;
                if (!string.IsNullOrEmpty(MarketsCheckboxName) && MarketsCheckboxName == "true")
                    checkBoxMarkets = true;
                else
                    checkBoxMarkets = false;

                string selectedMarketID = selectMarketID;
                int selectedPos = selectPos;

                bool checkBoxTerminal = false;
                if (!string.IsNullOrEmpty(TerminalsCheckboxName) && TerminalsCheckboxName == "true")
                    checkBoxTerminal = true;
                else
                    checkBoxTerminal = false;

                if (checkBoxMarkets || checkBoxTerminal)
                {
                    var cheques = await _chequeService.GetAllAsync(dateBegin, dateEnd, selectedMarketID, selectedPos, terminalID);

                    var csvResult = _chequeService.GetCSVReport(cheques.Data);

                    return csvResult.Success ? File(csvResult.Data.GetBuffer(), "text/csv", $"Отчёт-{DateBegin.ToString("yyyy_MM_dd")}-{DateEnd.ToString("yyyy_MM_dd")}.csv") : UnprocessableEntity(csvResult.Message);
                }
            }

            TempData["msg"] = "<script>alert('Выбранный период больше 1-го месяца, необходимо выбрать период не больше 1-го месяца!');</script>";

            return View();
        }
    }
}