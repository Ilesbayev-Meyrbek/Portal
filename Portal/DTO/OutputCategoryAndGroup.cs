namespace Portal.DTO
{
    public class OutputCategoryAndGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> GroupsPLU { get; set; }
    }
}