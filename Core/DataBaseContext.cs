using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Models;

namespace OrderDeliverySystem.Core
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options): base(options) { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItems>()
                .HasOne(o => o.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItems>()
                .HasOne(p=>p.Product)
                .WithMany()
                .HasForeignKey(p=>p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne(u => u.Vendor)
                .WithMany()
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users {get; set;}  
        public DbSet<Product> Products{get; set;}
        public DbSet<Order> Orders {get; set;}
        public DbSet<OrderItems> OrderItems { get; set; }
    }
}
