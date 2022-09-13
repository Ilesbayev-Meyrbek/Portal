using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Portal.DTO
{
    public class OutputMarketWithRole
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string MarketCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public IEnumerable<OutputScaleForMarket> Scales { get; set; }
    }
}