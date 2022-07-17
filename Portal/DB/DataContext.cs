using Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace Portal.DB
{
    public class DataContext : DbContext
    {
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
    }
}
