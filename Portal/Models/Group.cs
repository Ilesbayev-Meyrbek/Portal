using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Groups")]
    public class Group
    {
        public int Id { get; set; }
        public int Name { get; set; }

        public List<Good> Goods { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}