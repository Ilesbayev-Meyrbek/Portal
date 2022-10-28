using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table(nameof(ChequeBottomMessage))]

    public class ChequeBottomMessage
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Сообщение для чека")]
        [Required(ErrorMessage = "укажите сообщение для чека")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}
