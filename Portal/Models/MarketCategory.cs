using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("MarketCategories")]
    public class MarketCategory
    {
        public string MarketId { get; set; }
        public MarketsName Market { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int CategoryOrder { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}