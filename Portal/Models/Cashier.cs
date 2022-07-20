using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Cashiers")]
    public class Cashier
    {
        [Required(ErrorMessage = "Укажите пароль пользователя!")]
        [Display(Name = "Пароль")]
        [Key]
        [Column(Order = 0)]
        public string ID { get; set; }

        [Required(ErrorMessage = "Укажите имя пользователя!")]
        [Display(Name = "Кассир")]
        public string CashierName { get; set; }

        public string Password { get; set; }

        [Display(Name = "Админ(да/нет)")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Скидки (да/нет)")]
        public bool IsDiscounter { get; set; }

        public string TabelNumber { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IsGoodDisco { get; set; }
        public bool IsInvoicer { get; set; }
        public bool IsSaved { get; set; }
        public Int64 IsSavedToPOS { get; set; }
        public string IsSavedToMarket { get; set; }

        [Display(Name = "Маркет")]
        //[Key]
        [Column(Order = 1)]
        public string MarketID { get; set; }
    }
}