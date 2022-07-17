
namespace Portal.DTO
{
    public class ChequesDto : IEquatable<ChequesDto>
    {
        public DateTime Date { get; set; }
        public decimal SumCash { get; set; }
        public decimal SumNonCash { get; set; }
        public decimal TotalSum { get; set; }
        public decimal VAT { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountLoyal { get; set; }
        public int Cheque { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal VATReturn { get; set; }
        public decimal DiscountReturn { get; set; }
        public decimal DiscountLoyalReturn { get; set; }
        public int ChequeReturn { get; set; }
        public decimal VATTotal { get; set; }

        public bool Equals(ChequesDto other)
        {
            if (Date == other.Date && Date == other.Date)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashDate = Date == null ? 0 : Date.GetHashCode();

            return hashDate;
        }

    }
}