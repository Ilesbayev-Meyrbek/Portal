using Portal.DB;
using Portal.DTO;
using Portal.Services;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Portal.Controllers
{
    public class ReportsController : ApiController
    {
        private readonly DataContext _ctx;

        public ReportsController(DataContext ctx)
        {
            this._ctx = ctx;
        }

        // GET api/<controller>
        public HttpResponseMessage Post([System.Web.Http.FromBody] ReportDto dto)
        {
            if (dto == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            int DateBegin = int.Parse(dto.DateBegin.ToString("yyyyMMdd"));
            int DateEnd = int.Parse(dto.DateEnd.ToString("yyyyMMdd"));
            string MarketID = dto.MarketID;
            int? POSNum = dto.PosNum;
            string TerminalID = dto.TerminalID;

            if (DateEnd - DateBegin > 100)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            var preList = _ctx.Chequeses.Where(s => s.Date >= DateBegin && s.Date <= DateEnd);
            var newPreList = _ctx.NewCheques.Where(s => s.Date >= DateBegin && s.Date <= DateEnd);

            if (!string.IsNullOrEmpty(MarketID))
            {
                preList = preList.Where(m => m.MarketId == MarketID);
                newPreList = newPreList.Where(m => m.MarketId == MarketID);
            }
            if (!string.IsNullOrEmpty(TerminalID))
            {
                preList = preList.Where(s => s.TerminalId == TerminalID);
                newPreList = newPreList.Where(s => s.TerminalId == TerminalID);
            }
            if (POSNum != null && POSNum != 0)
            {
                preList = preList.Where(p => p.Posnum == POSNum);
                newPreList = newPreList.Where(p => p.Posnum == POSNum); ;
            }

            var NewCheque = (from f in newPreList
                             where f.TypeChar.Contains("+")
                             orderby f.Date ascending
                             group f by f.Date).ToList();

            var NewReturnCheque = (from f in newPreList
                                   where f.TypeChar.Contains("-")
                                   orderby f.Date ascending
                                   group f by f.Date).ToList();



            var Cheque = (from f in preList
                          where f.TypeChar.Contains("+")
                          orderby f.Date ascending
                          group f by f.Date).ToList();

            var ReturnCheque = (from f in preList
                                where f.TypeChar.Contains("-")
                                orderby f.Date ascending
                                group f by f.Date).ToList();


            var newList = (from c in NewCheque
                           join r in NewReturnCheque on c.Key equals r.Key into rk
                           from r in rk.DefaultIfEmpty()
                           select new ChequesDto()
                           {
                               Date = DateTime.ParseExact(c.Key.ToString(),
                                    "yyyyMMdd",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None).Date,
                               Cheque = (c?.Count() ?? 0),
                               SumCash = (c?.Sum(s => (decimal)s.TotalCash) ?? 0),
                               SumNonCash = (c?.Sum(s => (decimal)s.TotalNonCash) ?? 0),
                               TotalSum = ((c?.Sum(s => (decimal)s.TotalCash) ?? 0) + (c?.Sum(s => (decimal)s.TotalNonCash) ?? 0)),
                               VAT = (c?.Sum(s => (decimal)s.Vatsum) ?? 0),
                               TotalReturn = -1 * ((r?.Sum(s => (decimal)s.TotalNonCash) ?? 0) + (r?.Sum(s => (decimal)s.TotalCash) ?? 0)),
                               VATReturn = (r?.Sum(s => (decimal)s.Vatsum) ?? 0),
                               ChequeReturn = (r?.Count() ?? 0),
                               VATTotal = ((c?.Sum(s => (decimal)s.Vatsum) ?? 0) - (r?.Sum(s => (decimal)s.Vatsum) ?? 0))
                           }).ToList();

            foreach (var l in newList)
            {
                int ParsedDate = int.Parse(l.Date.ToString("yyyyMMdd"));

                l.Discount = ((decimal?)(from d in _ctx.ChequeGoodDiscounts
                                         let ChequeGoodIds = (from c in _ctx.ChequeGoods
                                                              let Ids = from f in newPreList
                                                                        where f.TypeChar.Contains("+")
                                                                        && f.Date == ParsedDate
                                                                        select f.Id
                                                              where Ids.Contains(c.ChequeId)
                                                              select c.Id)
                                         where !d.TypeDiscount.Contains("LOYALTY")
                                         && ChequeGoodIds.Contains(d.ChequeGoodId)
                                         select d.Value).DefaultIfEmpty().Sum() ?? 0);
                l.DiscountLoyal = ((decimal?)(from d in _ctx.ChequeGoodDiscounts
                                              let ChequeGoodIds = (from c in _ctx.ChequeGoods
                                                                   let Ids = from f in newPreList
                                                                             where f.TypeChar.Contains("+")
                                                                             && f.Date == ParsedDate
                                                                             select f.Id
                                                                   where Ids.Contains(c.ChequeId)
                                                                   select c.Id)
                                              where d.TypeDiscount.Contains("LOYALTY")
                                              && ChequeGoodIds.Contains(d.ChequeGoodId)
                                              select d.Value).DefaultIfEmpty().Sum() ?? 0);
                l.DiscountLoyalReturn = -1 * ((decimal?)(from d in _ctx.ChequeGoodDiscounts
                                                         let ChequeGoodIds = (from c in _ctx.ChequeGoods
                                                                              let Ids = from f in newPreList
                                                                                        where f.TypeChar.Contains("-")
                                                                                        && f.Date == ParsedDate
                                                                                        select f.Id
                                                                              where Ids.Contains(c.ChequeId)
                                                                              select c.Id)
                                                         where d.TypeDiscount.Contains("LOYALTY")
                                                         && ChequeGoodIds.Contains(d.ChequeGoodId)
                                                         select d.Value).DefaultIfEmpty().Sum() ?? 0) * -1;
                l.DiscountReturn = -1 * ((decimal?)(from d in _ctx.ChequeGoodDiscounts
                                                    let ChequeGoodIds = (from c in _ctx.ChequeGoods
                                                                         let Ids = from f in newPreList
                                                                                   where f.TypeChar.Contains("-")
                                                                                   && f.Date == ParsedDate
                                                                                   select f.Id
                                                                         where Ids.Contains(c.ChequeId)
                                                                         select c.Id)
                                                    where !d.TypeDiscount.Contains("LOYALTY")
                                                    && ChequeGoodIds.Contains(d.ChequeGoodId)
                                                    select d.Value).DefaultIfEmpty().Sum() ?? 0);
            }

            var list = (from c in Cheque
                        join r in ReturnCheque on c.Key equals r.Key into rk
                        from r in rk.DefaultIfEmpty()
                        select new ChequesDto()
                        {
                            Date = DateTime.ParseExact(c.Key.ToString(),
                                    "yyyyMMdd",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None).Date,
                            Cheque = c?.Count() ?? 0,
                            SumCash = c?.Sum(s => (decimal)s.TotalCash) ?? 0,
                            SumNonCash = c?.Sum(s => (decimal)s.TotalNonCash) ?? 0,
                            TotalSum = (c?.Sum(s => (decimal)s.TotalCash) ?? 0) + (c?.Sum(s => (decimal)s.TotalNonCash) ?? 0),
                            Discount = c?.Sum(s => (decimal)s.DiscountChegirma) ?? 0,
                            DiscountLoyal = c?.Sum(s => (decimal)s.DiscountBoshqa) ?? 0,
                            VAT = c?.Sum(s => (decimal)s.Vatsum) ?? 0,
                            TotalReturn = (r?.Sum(s => (decimal)s.TotalNonCash) ?? 0) + (r?.Sum(s => (decimal)s.TotalCash) ?? 0),
                            DiscountReturn = r?.Sum(s => (decimal)s.DiscountChegirma) ?? 0,
                            DiscountLoyalReturn = r?.Sum(s => (decimal)s.DiscountBoshqa) ?? 0,
                            VATReturn = r?.Sum(s => (decimal)s.Vatsum) ?? 0,
                            ChequeReturn = r?.Count() ?? 0,
                            VATTotal = (c?.Sum(s => (decimal)s.Vatsum) ?? 0) - (r?.Sum(s => (decimal)s.Vatsum) ?? 0)
                        }).ToList();

            var mergedList = newList.Concat(list).ToList();

            List<ChequesDto> unionList = new List<ChequesDto>();

            foreach (var l in mergedList)
            {
                unionList.Add(new ChequesDto()
                {
                    Date = l.Date,
                    Cheque = mergedList.Where(w => w.Date == l.Date).Sum(s => s.Cheque),
                    SumCash = mergedList.Where(w => w.Date == l.Date).Sum(s => s.SumCash),
                    SumNonCash = mergedList.Where(w => w.Date == l.Date).Sum(s => s.SumNonCash),
                    TotalSum = mergedList.Where(w => w.Date == l.Date).Sum(s => s.TotalSum),
                    Discount = mergedList.Where(w => w.Date == l.Date).Sum(s => s.Discount),
                    DiscountLoyal = mergedList.Where(w => w.Date == l.Date).Sum(s => s.DiscountLoyal),
                    VAT = mergedList.Where(w => w.Date == l.Date).Sum(s => s.VAT),
                    TotalReturn = mergedList.Where(w => w.Date == l.Date).Sum(s => s.TotalReturn),
                    DiscountReturn = mergedList.Where(w => w.Date == l.Date).Sum(s => s.DiscountReturn),
                    DiscountLoyalReturn = mergedList.Where(w => w.Date == l.Date).Sum(s => s.DiscountLoyalReturn),
                    VATReturn = mergedList.Where(w => w.Date == l.Date).Sum(s => s.VATReturn),
                    ChequeReturn = mergedList.Where(w => w.Date == l.Date).Sum(s => s.ChequeReturn),
                    VATTotal = mergedList.Where(w => w.Date == l.Date).Sum(s => s.VATTotal)
                });
            }

            var finallist = unionList.Distinct().OrderBy(f => f.Date).ToList();


            string[] header = new[] { "№", "Дата", "Сумма(нал)", "Сумма(безнал)", "Итого с НДС", "НДС",
            "Сумма скидки", "Скидка по картам лояльности", "Чеки",
            "Сумма возврата с НДС", "НДС возврата", "Возврат по скидкам", "Возврат по картам лояльности",
            "Чеки возврата", "НДС итоговый"};




            var csv = new ListToCSV<ChequesDto>(finallist, header);

            var file = csv.CreateCSV();
            var ms = new MemoryStream(file);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Отчёт_" + DateBegin + "-" + DateEnd + ".csv";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

            return response;

            ms.Dispose();
        }

        // GET api/<controller>/5
        [System.Web.Http.Route("api/reports/GetPosByMarket/{MarketId:string}")]
        [System.Web.Http.HttpGet]
        public List<int> GetPosByMarket(string MarketId)
        {
            var posList = (_ctx.Chequeses.Where(x => x.MarketId == MarketId).Select(x => x.Posnum)).Union(_ctx.NewCheques.Where(x => x.MarketId == MarketId).Select(x => x.Posnum)).Distinct().ToList();
            posList.Sort();
            return posList;//Json(posList);
        }

    }
}
