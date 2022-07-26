using Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace Portal.DB
{
    public class DataContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MarketsName> Markets { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<Keyboard> Keyboards { get; set; }
        public DbSet<SettingsKey> SettingsKeys { get; set; }














        public DbSet<Role> Roles { get; set; }








        public DbSet<Cheques> Chequeses { get; set; }
        public DbSet<NewCheque> NewCheques { get; set; }
        public DbSet<ChequeGood> ChequeGoods { get; set; }
        public DbSet<ChequeGoodDiscount> ChequeGoodDiscounts { get; set; }

        public DbSet<Good> Goods { get; set; }
        public DbSet<GoodsDetail> GoodsDetails { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Cashier>().HasKey(table => new {
                table.ID,
                table.MarketID
            });
        }
    }
}
