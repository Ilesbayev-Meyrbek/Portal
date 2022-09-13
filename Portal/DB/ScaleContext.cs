using Microsoft.EntityFrameworkCore;
using Portal.Models;

namespace Portal.DB
{
    public class ScaleContext : DbContext
    {
        public DbSet<Scale> Scales { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<ScalesGood> ScalesGoods { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ScalesSyncStatus> ScalesSyncStatuses { get; set; }

        public ScaleContext(DbContextOptions<ScaleContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Scale>().HasKey(table => new { table.ID });

            builder.Entity<MarketCategory>()
            .HasKey(sc => new { sc.MarketId, sc.CategoryId });
        }
    }
}
