using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("ChequeGood")]
    public class ChequeGood
    {
        public long Id { get; set; }
        public long ChequeId { get; set; }
        public int Number { get; set; }
        public string Code { get; set; }
        public string Sapid { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Sum { get; set; }
        public decimal Vatrate { get; set; }
        public decimal Vatsum { get; set; }
        public int UnitCode { get; set; }
        public bool? HasLabel { get; set; }
        public string Label { get; set; }
        public string Tin { get; set; }
        public string Pinfl { get; set; }
        public string MeasureUnit { get; set; }
        public string ClassCode { get; set; }

    }
}
