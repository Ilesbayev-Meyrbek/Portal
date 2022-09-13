using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }
        public string RuName { get; set; }
        public string EnName { get; set; }
        public string UzName { get; set; }
        public DateTime UpdateTime { get; set; }

        public List<Group> Groups { get; set; }

        public List<MarketCategory> Scales { get; set; }
    }
}