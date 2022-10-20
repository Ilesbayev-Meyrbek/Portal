using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.DTO
{
    public class LogoView
    {
        public List<Logo> Logos { get; set; }
        public bool IsAdmin { get; set; }
        public Roles UserRole { get; set; }
        public List<MarketsName> Markets { get; set; }
    }
}