using Microsoft.EntityFrameworkCore;
using AiraaFlorals.Models;


namespace AiraaFlorals.Models
{
    public class FloralsContext : DbContext
    {
        // constructor
        public FloralsContext()
        {

        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Occasion> Occasions { get; set; }

        public DbSet<Bouquets> Bouquets { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bouquets>()
            .HasOne(b => b.Occasion)
            .WithMany(g => g.Bouquets)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
            .HasOne(b => b.Cart)
            .WithMany(g => g.CartItems)
            .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, CustomerName = "Admin", UserName="admin", Password="admin123" }
            );

        }

        public FloralsContext(DbContextOptions<FloralsContext> options) : base(options)
        {

        }

    }
}
