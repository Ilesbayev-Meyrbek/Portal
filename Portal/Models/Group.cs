using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Groups")]
    public class Group
    {
        public int Id { get; set; }
        public string SapID { get; set; }

        public List<ScalesGood> Goods { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}