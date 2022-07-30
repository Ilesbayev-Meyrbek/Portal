namespace Portal.Models
{
    public class LogoView
    {
        public List<Logo> Logos { get; set; }
        public bool IsAdmin { get; set; }
        public Role UserRole { get; set; }
        public List<MarketsName> Markets { get; set; }
    }
}