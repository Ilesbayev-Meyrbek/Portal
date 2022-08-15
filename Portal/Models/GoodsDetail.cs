using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("GoodsDetails")]
    public class GoodsDetail
    {
        public long ID { get; set; }
        public long GoodsID { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public int PeriodBeg { get; set; }
        public int PeriodEnd { get; set; }
        public string Pricemode { get; set; }
        public string ActionNum { get; set; }
        public DateTime ServerDateTime { get; set; }

        public virtual Good Goods { get; set; }
    }
}