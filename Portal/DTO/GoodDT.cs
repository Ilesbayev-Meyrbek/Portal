namespace Portal.DTO
{
    public class GoodDT
    {
        public long ID { get; set; }
        public string MarketID { get; set; }
        public bool IsSaved { get; set; }
        public long GoodsID { get; set; }
        public DateTime ServerDateTime { get; set; }
    }
}