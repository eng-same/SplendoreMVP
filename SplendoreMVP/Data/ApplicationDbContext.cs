using Microsoft.EntityFrameworkCore;
using SplendoreMVP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SplendoreMVP.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) 
        {
            
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - Cart (1:1)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.Customer)
                .HasForeignKey<Cart>(c => c.CustomerID);

            // User - Orders (1:N)
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerID);

            // Category - Products (1:N)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryID);

            // Order - OrderItems (1:N)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderID);

            // Cart - CartItems (1:N)
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartID);

            // Product - OrderItems (N:1)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductID);

            // Product - CartItems (N:1)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductID);
        }
    }
}
