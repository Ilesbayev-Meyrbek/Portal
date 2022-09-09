using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Logo")]
    public class Logo
    {
        public int ID { get; set; }
        public string MarketID { get; set; }
        public byte[] BMP { get; set; }
        public int DateBegin { get; set; }
        public int DateEnd { get; set; }
        public string? Note { get; set; }
        public long IsSavedToPOS { get; set; }
        public bool IsSaved { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy'/'MM'/'dd}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "Укажите дату начала!")]
        public DateTime DateS { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy'/'MM'/'dd}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "Укажите дату завершения!")]
        public DateTime DateE { get; set; }

        //public string Name { get; set; }
        //public byte[] Avatar { get; set; }
    }
}