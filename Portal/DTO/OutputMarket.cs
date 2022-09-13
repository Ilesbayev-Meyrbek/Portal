using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.DTO
{
    public class OutputMarket
    {
        public string Id { get; set; }
        public string MarketCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public IEnumerable<OutputScaleForMarket> Scales { get; set; }
    }
}