using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Portal.DTO
{
    public class GoodsForScale
    {
        public Int64 ID { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string SAPID { get; set; }
        public string Code { get; set; }
        public string Weight { get; set; }
        public string PLU { get; set; }
        public int? ImageId { get; set; }
        public int? GroupId { get; set; }
        public int? CategoryId { get; set; }
    }
}