using System.Collections.Generic;

namespace Portal.Models
{
    public class ScaleView
    {
        public List<Scale> Scales { get; set; }
        public bool IsAdmin { get; set; }
        public Role UserRole { get; set; }
        public string Market { get; set; }
        public List<MarketsName> Markets { get; set; }
    }
}