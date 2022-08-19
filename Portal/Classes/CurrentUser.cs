using Portal.Models;

namespace Portal.Classes
{
    public class CurrentUser
    {
        public string Login { get; set; }
        public string MarketID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUser { get; set; }
        public Role Roles { get; set; }
    }
}
