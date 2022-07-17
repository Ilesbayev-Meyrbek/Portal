using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("ChequeGoodDiscount")]
    public class ChequeGoodDiscount
    {
        public long Id { get; set; }
        public long ChequeGoodId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        [Column("Type")]
        public string TypeDiscount { get; set; }
        public string ActionId { get; set; }
        public string VouchType { get; set; }
        public string VoucherCode { get; set; }
        public string BonusBuyType { get; set; }
        public int BonusBuyId { get; set; }
    }
}
