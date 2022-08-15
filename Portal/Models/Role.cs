using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("Roles")]
    public class Role
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Укажите роль!")]
        [Display(Name = "Наименование роли")]
        public string Name { get; set; }

        public bool AllMarkets { get; set; }

        public bool CreateCashiers { get; set; }
        public bool EditCashiers { get; set; }
        public bool DeleteCashiers { get; set; }

        public bool CreateLogo { get; set; }
        public bool EditLogo { get; set; }
        public bool DeleteLogo { get; set; }

        public bool CreateSettings { get; set; }
        public bool EditSettings { get; set; }
        public bool DeleteSettings { get; set; }

        public bool CreateKeyboard { get; set; }
        public bool EditKeyboard { get; set; }
        public bool DeleteKeyboard { get; set; }

        public bool AdminForScale { get; set; }

        public bool Scales { get; set; }
        public bool POSs { get; set; }
    }
}