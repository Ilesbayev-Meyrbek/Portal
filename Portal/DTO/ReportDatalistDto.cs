using Microsoft.AspNetCore.Mvc.Rendering;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.DTO
{
    public class ReportDatalistDto
    {
        public List<MarketsName> MarketId { get; set; }
        public IEnumerable<SelectListItem> SelectListOfMarkets { get; set; }
        public List<int> Pos { get; set; }
        public IEnumerable<SelectListItem> SelectListOfPos { get; set; }
    }
}