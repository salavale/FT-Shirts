using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<CatalogueItem>().HasKey(c => c.ShirtId);
            modelBuilder.Entity<CatalogueItem>().HasMany(g => g.Tags).WithMany();
        }

        public static void Configure(DbContextOptionsBuilder options, string connectionString)
        {
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention();
        }
    }
}