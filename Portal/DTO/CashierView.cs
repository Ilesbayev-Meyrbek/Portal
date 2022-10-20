
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.DTO
{
    public class CashierView
    {
        public List<Cashiers> Cashiers { get; set; }
        public bool IsAdmin { get; set; }
        public Roles UserRole { get; set; }
        public List<MarketsName> Markets { get; set; }
    }
}