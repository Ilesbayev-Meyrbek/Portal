using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Cheques")]
    public partial class Cheques
    {
        public string MarketId { get; set; }
        [Key]
        public string ChqId { get; set; }
        public int Date { get; set; }
        public int Time { get; set; }
        public int Number { get; set; }

        [Column("Type")]
        public string TypeChar { get; set; }
        public decimal Total { get; set; }
        public string RetChqId { get; set; }
        public string CashierId { get; set; }
        public string CashierName { get; set; }
        public int Fiscal { get; set; }
        public bool IsSaved { get; set; }
        public int WorkDate { get; set; }
        public string Lcard { get; set; }
        public int InvoiceNum { get; set; }
        public int Posnum { get; set; }
        public int TimeOpen { get; set; }
        public bool? IsSecond { get; set; }
        public decimal? TotalCash { get; set; }
        public decimal? TotalNonCash { get; set; }
        public decimal? Vatsum { get; set; }
        public decimal? Vatcorrection { get; set; }
        public decimal? DiscountChegirma { get; set; }
        public decimal? DiscountBoshqa { get; set; }
        public string TerminalId { get; set; }
        public string ReceiptSeq { get; set; }
        public string FiscalSign { get; set; }
    }
}
