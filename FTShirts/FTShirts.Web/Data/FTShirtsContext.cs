using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.CartModels;

namespace FTShirts.Web.Data
{
    public class FTShirtsContext : DbContext
    {
        public FTShirtsContext(DbContextOptions<FTShirtsContext> options) : base(options)
        {
        }

        public DbSet<CatalogueItem> CatalogueItems => Set<CatalogueItem>();
        public DbSet<Shirt> Shirts => Set<Shirt>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);

            model.Entity<User>().HasKey(u => u.Username);
            model.Entity<CatalogueItem>().HasKey(c => c.ShirtId);

            model.Entity<ShoppingCart>().HasKey(s => s.Username);

            model.Entity<User>().HasOne(u => u.ShoppingCart).WithOne().HasForeignKey<ShoppingCart>(s => s.Username);
        }

        public static void Configure(DbContextOptionsBuilder options, string connectionString)
        {
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention();
        }
    }
}