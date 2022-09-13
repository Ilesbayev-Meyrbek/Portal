
namespace Portal.DTO
{
    public class CategoriesForScale
    {
        public int CategoryId { get; set; }
        public string CategoryRuName { get; set; }
        public string CategoryEnName { get; set; }
        public string CategoryUzName { get; set; }
        public int CategoryOrder { get; set; }
        public bool isDefault { get; set; }
    }
}