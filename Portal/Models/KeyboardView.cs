namespace Portal.Models
{
    public class KeyboardView
    {
        public List<Keyboard> Keyboards { get; set; }
        public bool IsAdmin { get; set; }
        public Role UserRole { get; set; }
        public List<MarketsName> Markets { get; set; }
    }
}