using System.ComponentModel.DataAnnotations;

namespace Portal.Models
{
    public partial class ChequeGoodDiscount
    {
        [Key]   
        public long ID { get; set; }

        public long ChequeGoodID { get; set; }

        public decimal Quantity { get; set; }

        public decimal Value { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [Required]
        [StringLength(20)]
        public string ActionID { get; set; }

        [Required]
        [StringLength(20)]
        public string VouchType { get; set; }

        [Required]
        [StringLength(20)]
        public string VoucherCode { get; set; }

        [Required]
        [StringLength(20)]
        public string BonusBuyType { get; set; }

        public int BonusBuyID { get; set; }

        public virtual ChequeGood ChequeGood { get; set; }
    }
}
