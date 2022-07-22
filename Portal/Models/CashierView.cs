namespace Portal.Models
{
    public class CashierView
    {
        public List<Cashier> Cashiers { get; set; }
        public bool IsAdmin { get; set; }
        public Role UserRole { get; set; }
        public List<MarketsName> Markets { get; set; }
    }
}