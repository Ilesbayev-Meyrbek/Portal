
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.DTO
{
    public class KeyboardView
    {
        public List<POSSettingsKeyboard> Keyboards { get; set; }
        public bool IsAdmin { get; set; }
        public Roles UserRole { get; set; }
        public List<MarketsName> Markets { get; set; }
    }
}