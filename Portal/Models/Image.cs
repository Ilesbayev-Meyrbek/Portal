namespace Portal.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Thumbnail { get; set; }
        public DateTime UpdateTime { get; set; }

        public List<ScalesGood> Goods { get; set; }
    }
}