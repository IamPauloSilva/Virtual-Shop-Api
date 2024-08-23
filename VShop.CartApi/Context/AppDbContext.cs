using Microsoft.EntityFrameworkCore;
using VShop.CartApi.Models;

namespace VShop.CartApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<CartProduct>? CartProducts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<CartHeader> CartHeaders { get; set; }

    
    protected override void OnModelCreating(ModelBuilder mb)
    {
        
        mb.Entity<CartProduct>()
            .HasKey(c => c.Id);

        
        mb.Entity<CartProduct>().
           Property(c => c.Id)
            .ValueGeneratedNever();

        mb.Entity<CartProduct>().
           Property(c => c.Name).
             HasMaxLength(100).
               IsRequired();

        mb.Entity<CartProduct>().
          Property(c => c.Description).
               HasMaxLength(255).
                   IsRequired();

        mb.Entity<CartProduct>().
          Property(c => c.ImageURL).
              HasMaxLength(255).
                  IsRequired();

        mb.Entity<CartProduct>().
           Property(c => c.CategoryName).
               HasMaxLength(100).
                IsRequired();

        mb.Entity<CartProduct>().
           Property(c => c.Price).
             HasPrecision(12, 2);

        
        mb.Entity<CartHeader>().
             Property(c => c.UserId).
             HasMaxLength(255).
                 IsRequired();

        mb.Entity<CartHeader>().
           Property(c => c.CouponCode).
              HasMaxLength(100);
    }
}
