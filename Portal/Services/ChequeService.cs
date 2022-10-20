using Portal.CacheManager;
using Portal.Services.Interfaces;
using Portal.Repositories.Interfaces;
using UZ.STS.POS2K.DataAccess.Models;
using CsvHelper.Configuration;
using CsvHelper;
using Portal.DTO;
using System.Globalization;
using System.Linq.Expressions;
using Portal.Extensions;
using Cheque = UZ.STS.POS2K.DataAccess.Models.Cheque;
using ChequeGood = UZ.STS.POS2K.DataAccess.Models.ChequeGood;
using System.Text;

namespace Portal.Services
{
    public class ChequeService : IChequeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChequeService> _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IChequeGoodService _chequeGoodService;
        private readonly IChequeGoodDiscountService _chequeGoodDiscountService;

        public ChequeService(IUnitOfWork unitOfWork,
            IChequeGoodService chequeGoodService,
            IChequeGoodDiscountService chequeGoodDiscountService,
            ICacheManager cacheManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChequeService> logger)
        {
            _unitOfWork = unitOfWork;
            _chequeGoodService = chequeGoodService;
            _chequeGoodDiscountService = chequeGoodDiscountService;
            _cacheManager = cacheManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Result<List<string>>> GetAllTerminalIDAsync()
        {
            try
            {
                var chequeTerminalIDs = await _unitOfWork.Cheques.GetAllAsync(u => !string.IsNullOrEmpty(u.TerminalID));

                var terminalIDs = chequeTerminalIDs.GroupBy(x => x.TerminalID).Select(x => x.Key).ToList();

                return Result<List<string>>.Ok(terminalIDs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Result<List<string>>.Failed(ex.Message);
            }

        }

        public async Task<Result<List<AccountantReport>>> GetAllAsync(int dateBegin, int dateEnd, string marketID, int posNumber, string terminalID)
        {
            try
            {
                Expression<Func<Cheque, bool>> predicate =
                    g => dateBegin <= g.Date &&
                    g.Date <= dateEnd &&
                    (g.Type == "+" || g.Type == "-");

                if (!string.IsNullOrEmpty(terminalID))
                    terminalID = terminalID.Trim();

                if (!string.IsNullOrEmpty(marketID))
                {
                    predicate = predicate.And(g => g.MarketID == marketID);

                    if (posNumber != 0)
                        predicate = predicate.And(g => g.POSNum == posNumber);
                }
                else if (!string.IsNullOrEmpty(terminalID))
                {
                    predicate = predicate.And(g => g.TerminalID == terminalID);
                }

                var include = new[] {
                    $"{nameof(Cheque.ChequeGood)}",
                    $"{nameof(Cheque.ChequeGood)}.{nameof(ChequeGood.ChequeGoodDiscount)}"};

                var cheques = await _unitOfWork.Cheques.GetAllWithStringIncludeAsync(predicate, include);

                List<AccountantReport> accountantSaleReports = new List<AccountantReport>();

                foreach (var cheque in cheques)
                {
                    AccountantReport accountantReport = new AccountantReport();

                    accountantReport.Date = cheque.Date.ToString();

                    if (cheque.Type == "+")
                    {
                        accountantReport.AmountCash = cheque.TotalCash;
                        accountantReport.AmountNonCash = cheque.TotalNonCash;
                        accountantReport.TotalWithVAT = cheque.TotalCash + cheque.TotalNonCash;
                        accountantReport.VAT = cheque.VATSum;
                        accountantReport.DiscountAmount = cheque.ChequeGood.Sum(w => w.ChequeGoodDiscount.Where(d => d.Type != "LOYALTY").Sum(s => s.Value));
                        accountantReport.LoyaltyCardDiscount = cheque.ChequeGood.Sum(w => w.ChequeGoodDiscount.Where(d => d.Type == "LOYALTY").Sum(s => s.Value));
                        accountantReport.SalesChequeCount += 1;

                        accountantSaleReports.Add(accountantReport);
                    }
                    else if (cheque.Type == "-")
                    {
                        accountantReport.VATRefundAmount = cheque.TotalCash + cheque.TotalNonCash;
                        accountantReport.VATRefund = cheque.VATSum;
                        accountantReport.ReturnDiscountAmount = cheque.ChequeGood.Sum(w => w.ChequeGoodDiscount.Where(d => d.Type != "LOYALTY").Sum(s => s.Value));
                        accountantReport.ReturnLoyaltyCardDiscount = cheque.ChequeGood.Sum(w => w.ChequeGoodDiscount.Where(d => d.Type == "LOYALTY").Sum(s => s.Value));
                        accountantReport.ReturnChequeCount += 1;

                        accountantSaleReports.Add(accountantReport);
                    }
                }

                int index = 0;

                var report = accountantSaleReports.GroupBy(g => g.Date).Select(s => new AccountantReport
                {
                    Number = ++index,
                    Date = s.Key,                                                            // Дата 
                    AmountCash = s.Sum(w => w.AmountCash),                                   // Сумма(нал)
                    AmountNonCash = s.Sum(w => w.AmountNonCash),                             // Сумма(безнал)
                    TotalWithVAT = s.Sum(w => w.TotalWithVAT),                               // Итого с НДС
                    VAT = s.Sum(w => w.VAT),                                                 // НДС
                    DiscountAmount = s.Sum(w => w.DiscountAmount),
                    LoyaltyCardDiscount = s.Sum(w => w.LoyaltyCardDiscount),
                    SalesChequeCount = s.Sum(w => w.SalesChequeCount),                       // Чеки продаж

                    VATRefundAmount = s.Sum(w => w.VATRefundAmount),                         // Сумма возарата с НДС
                    VATRefund = s.Sum(w => w.VATRefund),                                     // НДС возврата
                    VATFinal = s.Sum(w => w.VAT) - s.Sum(w => w.VATRefund),                  // НДС итоговый
                    ReturnDiscountAmount = s.Sum(w => w.ReturnDiscountAmount),               // Сумма скидки возврата
                    ReturnLoyaltyCardDiscount = s.Sum(w => w.ReturnLoyaltyCardDiscount),     // Скидка по картам лояльности возврата
                    ReturnChequeCount = s.Sum(w => w.ReturnChequeCount)                      // Чеки возврата
                }).ToList();

                var totalReport = new AccountantReport
                {
                    Date = "Итого:",
                    AmountCash = report.Sum(w => w.AmountCash),
                    AmountNonCash = report.Sum(w => w.AmountNonCash),                             // Сумма(безнал)
                    TotalWithVAT = report.Sum(w => w.TotalWithVAT),                               // Итого с НДС
                    VAT = report.Sum(w => w.VAT),                                                 // НДС
                    DiscountAmount = report.Sum(w => w.DiscountAmount),
                    LoyaltyCardDiscount = report.Sum(w => w.LoyaltyCardDiscount),
                    SalesChequeCount = report.Sum(w => w.SalesChequeCount),                       // Чеки продаж

                    VATRefundAmount = report.Sum(w => w.VATRefundAmount),                         // Сумма возарата с НДС
                    VATRefund = report.Sum(w => w.VATRefund),                                     // НДС возврата
                    VATFinal = report.Sum(w => w.VAT) - report.Sum(w => w.VATRefund),             // НДС итоговый
                    ReturnDiscountAmount = report.Sum(w => w.ReturnDiscountAmount),               // Сумма скидки возврата
                    ReturnLoyaltyCardDiscount = report.Sum(w => w.ReturnLoyaltyCardDiscount),     // Скидка по картам лояльности возврата
                    ReturnChequeCount = report.Sum(w => w.ReturnChequeCount)                      // Чеки возврата
                };

                report.Add(totalReport);

                return Result<List<AccountantReport>>.Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result<List<AccountantReport>>.Failed(ex.Message);
            }
        }

        public Result<MemoryStream> GetCSVReport(List<AccountantReport> report)
        {
            try
            {
                if (report.Count > 0)
                {
                    var configPersons = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true,
                        Delimiter = ";"
                    };

                    MemoryStream ms = new MemoryStream();

                    using (var writer = new StreamWriter(ms, Encoding.UTF8))
                    {
                        using (var csv = new CsvWriter(writer, configPersons))
                        {
                            csv.WriteRecords(report.OrderBy(x => x.Date).ToList());
                        }

                        return Result<MemoryStream>.Ok(ms);
                    }
                }

                return Result<MemoryStream>.Failed("Нет данных");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return Result<MemoryStream>.Failed(ex.Message);
            }
        }
    }
}