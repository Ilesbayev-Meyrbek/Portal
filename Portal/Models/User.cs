using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Маркет")]
        [Required(ErrorMessage = "Укажите маркет пользователя!")]
        public string MarketID { get; set; }
        public MarketsName Market { get; set; }

        [Display(Name = "Ф.И.О.")]
        [Required(ErrorMessage = "Укажите Ф.И.О. пользователя!")]
        public string Name { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Укажите логин пользователя!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Укажите роль пользователя!")]
        public int RoleID { get; set; }

        public Role Role { get; set; }
        
    }
}