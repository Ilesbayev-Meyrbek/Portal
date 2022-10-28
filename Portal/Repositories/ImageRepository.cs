using Portal.DB;
using Portal.Models;
using Portal.Repositories.Interfaces;


namespace Portal.Repositories
{
    public class ImageRepository : BaseScaleRepository<Image>, IImageRepository
    {
        private readonly ScaleContext _context;

        public ImageRepository(ScaleContext context) : base(context)
        {
            _context = context;
        }
    }
}