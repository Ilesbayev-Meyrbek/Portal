using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("GoodsScalePrices")]
    public class ScalesGood
    {
        public Int64 ID { get; set; }
        public string MarketID { get; set; }
        public string PLU { get; set; }
        public string SAPID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Weight { get; set; }
        public string CreateGoodDate { get; set; }
        public DateTime ReceiptGoodDate { get; set; }
        public bool IsSavedToTiger { get; set; }
        public bool IsSavedToBPlus { get; set; }
        public bool IsSavedToFB { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public int? ImageId { get; set; }
        public Image Image { get; set; }
    }
}