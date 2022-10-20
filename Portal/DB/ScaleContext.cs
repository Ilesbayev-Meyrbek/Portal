using Microsoft.EntityFrameworkCore;
using Portal.ScaleModels;

namespace Portal.DB
{
    public class ScaleContext : DbContext
    {
        public DbSet<Scale> Scales { get; set; }

        public ScaleContext(DbContextOptions<ScaleContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Scale>().HasKey(table => new { table.ID });
        }
    }
}
