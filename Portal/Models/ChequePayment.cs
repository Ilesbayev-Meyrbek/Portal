using System.ComponentModel.DataAnnotations;

namespace Portal.Models
{
    public partial class ChequePayment
    {
        public long ID { get; set; }

        public long ChequeID { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        public decimal Value { get; set; }

        public virtual Cheque Cheque { get; set; }
    }
}
