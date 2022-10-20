
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.DTO
{
    public class POSView
    {
        public List<GoodStatus> Items { get; set; }
        public bool IsAdmin { get; set; }
        public Roles UserRole { get; set; }
        public string Market { get; set; }
        public List<MarketsName> Markets { get; set; }
    }

    public class GoodStatus
    {
        public string MarketID { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}