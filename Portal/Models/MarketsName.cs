using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("MarketsName")]
    public class MarketsName
    {
        [Key]
        public string MarketID { get; set; }
        public string Name { get; set; }
        public string? POS { get; set; }
        public string? POSVersion { get; set; }
        public bool FilesLoaded { get; set; }
        
        //public virtual ICollection<User> Users { get; set; }
    }
}