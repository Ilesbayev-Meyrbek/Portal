using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Admin")]
    public class Admin
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
    }
}