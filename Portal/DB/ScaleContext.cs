using Microsoft.EntityFrameworkCore;

namespace Portal.DB
{
    public class ScaleContext : DbContext
    {
        public ScaleContext(DbContextOptions<ScaleContext> options)
           : base(options)
        {
        }
    }
}
