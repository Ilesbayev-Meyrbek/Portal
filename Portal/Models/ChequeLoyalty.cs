using System.ComponentModel.DataAnnotations;

namespace Portal.Models
{
    public partial class ChequeLoyalty
    {
        [Key]
        public long ChequeID { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [Required]
        [StringLength(20)]
        public string PseudoCode { get; set; }

        public virtual Cheque Cheque { get; set; }
    }
}
