namespace MusicalZed.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using MusicalZed.Domain.Entities;

public class MusicalZedDbContext(DbContextOptions<MusicalZedDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");
            e.Property(p => p.OriginalPrice).HasColumnType("decimal(10,2)");
            e.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
            e.HasIndex(p => p.CategoryId);
            e.HasIndex(p => p.IsActive);
        });

        modelBuilder.Entity<CartItem>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.UnitPrice).HasColumnType("decimal(10,2)");
            e.HasOne(c => c.Product).WithMany().HasForeignKey(c => c.ProductId);
            e.HasIndex(c => c.SessionId);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Subtotal).HasColumnType("decimal(10,2)");
            e.Property(o => o.ShippingCost).HasColumnType("decimal(10,2)");
            e.Property(o => o.Total).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(oi => oi.Id);
            e.Property(oi => oi.UnitPrice).HasColumnType("decimal(10,2)");
            e.Property(oi => oi.Total).HasColumnType("decimal(10,2)");
            e.HasOne(oi => oi.Order).WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);
            e.HasOne(oi => oi.Product).WithMany().HasForeignKey(oi => oi.ProductId);
        });
    }
}
