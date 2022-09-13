namespace Portal.DTO
{
    public class OutputGood
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PLU { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int? GroupPLU { get; set; } = 0;
        public int? ImageId { get; set; }
        public string Thumbnail { get; set; }
    }
}