using System.ComponentModel.DataAnnotations;

namespace Portal.Models
{ 
    public partial class ChequeGoodCancel
    {
        [Key]
        public long ID { get; set; }

        public long ChequeID { get; set; }

        public int Number { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [Required]
        [StringLength(15)]
        public string SAPID { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public decimal Sum { get; set; }

        public decimal VATRate { get; set; }

        public decimal VATSum { get; set; }

        public int UnitCode { get; set; }

        public bool? HasLabel { get; set; }

        [Required]
        public string Label { get; set; }

        [Required]
        [StringLength(20)]
        public string TIN { get; set; }

        [Required]
        [StringLength(20)]
        public string PINFL { get; set; }

        [Required]
        [StringLength(10)]
        public string MeasureUnit { get; set; }

        [Required]
        [StringLength(30)]
        public string ClassCode { get; set; }

        public virtual Cheque Cheque { get; set; }
    }
}
