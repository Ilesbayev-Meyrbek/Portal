using System.ComponentModel.DataAnnotations;

namespace Portal.Models
{
    public partial class Cheque
    {
        public Cheque()
        {
            ChequeGood = new HashSet<ChequeGood>();
            ChequeGoodCancel = new HashSet<ChequeGoodCancel>();
            ChequePayment = new HashSet<ChequePayment>();
        }

        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(30)]
        public string ChqId { get; set; }

        [Required]
        [StringLength(4)]
        public string MarketID { get; set; }

        public int POSNum { get; set; }

        public int WorkDate { get; set; }

        public int Date { get; set; }

        public int TimeOpen { get; set; }

        public int TimeClose { get; set; }

        public int Number { get; set; }

        [Required]
        [StringLength(1)]
        public string Type { get; set; }

        public decimal Total { get; set; }

        public decimal TotalCash { get; set; }

        public decimal TotalNonCash { get; set; }

        public decimal VATSum { get; set; }

        public decimal VATCorrection { get; set; }

        [Required]
        [StringLength(30)]
        public string RetChqId { get; set; }

        [Required]
        [StringLength(30)]
        public string CashierId { get; set; }

        [Required]
        [StringLength(30)]
        public string CashierName { get; set; }

        [StringLength(14)]
        public string TerminalID { get; set; }

        [StringLength(30)]
        public string DateTime { get; set; }

        [StringLength(9)]
        public string ReceiptSeq { get; set; }

        [StringLength(12)]
        public string FiscalSign { get; set; }

        [Required]
        public string QRCodeUrl { get; set; }

        public int Fiscal { get; set; }

        public bool IsSecond { get; set; }

        public bool IsSaved { get; set; }

        public DateTime ServerDateTime { get; set; }

        public virtual ICollection<ChequeGood> ChequeGood { get; set; }

        public virtual ICollection<ChequeGoodCancel> ChequeGoodCancel { get; set; }

        public virtual ChequeLoyalty ChequeLoyalty { get; set; }

        public virtual ICollection<ChequePayment> ChequePayment { get; set; }
    }
}
