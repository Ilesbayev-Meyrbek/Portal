using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("SettingsKeys")]
    public class SettingsKey
    {
        public int ID { get; set; }
        public string KeyCode { get; set; }
        public string Value { get; set; }
    }
}