using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Goods")]
    public class Good
    {
        public Good()
        {
            GoodsDetails = new HashSet<GoodsDetail>();
        }

        public Int64 ID { get; set; }
        public string MarketID { get; set; }
        public string SAPID { get; set; }
        public bool Weight { get; set; }
        public decimal VAT { get; set; }
        public string Name { get; set; }
        public string ClassCodeSAP { get; set; }
        public string ClassCodeOFD { get; set; }
        public string MeasureUnit { get; set; }
        public string PLU { get; set; }
        public bool Status { get; set; }
        public bool IsSavedToPricer { get; set; }
        public Int64 IsSavedToPOS { get; set; }
        public bool IsSaved { get; set; }
        public int UnitCode { get; set; }
        public string UnitName { get; set; }
        public bool HasLabel { get; set; }
        public string CTIN { get; set; }

        public virtual ICollection<GoodsDetail> GoodsDetails { get; set; }
    }
}