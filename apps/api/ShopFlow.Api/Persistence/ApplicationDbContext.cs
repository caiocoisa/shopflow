using Microsoft.EntityFrameworkCore;
using ShopFlow.Api.Domain;

namespace ShopFlow.Api.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Name).IsRequired().HasMaxLength(128);
            e.HasOne(u => u.Cart).WithOne(c => c.User!).HasForeignKey<Cart>(c => c.UserId);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
            e.Property(c => c.Name).IsRequired().HasMaxLength(128);
            e.Property(c => c.Slug).IsRequired().HasMaxLength(128);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Name).IsRequired().HasMaxLength(256);
            e.Property(p => p.Description).HasMaxLength(2000);
            e.Property(p => p.Price).HasColumnType("numeric(12,2)");
            e.Property(p => p.ImageUrl).HasMaxLength(512);
            e.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(p => p.CategoryId);
        });

        modelBuilder.Entity<Cart>(e =>
        {
            e.HasMany(c => c.Items).WithOne(i => i.Cart!).HasForeignKey(i => i.CartId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(e =>
        {
            e.Property(i => i.UnitPrice).HasColumnType("numeric(12,2)");
            e.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(i => new { i.CartId, i.ProductId }).IsUnique();
            e.Ignore(i => i.Subtotal);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.Property(o => o.Total).HasColumnType("numeric(12,2)");
            e.Property(o => o.StripeSessionId).HasMaxLength(256);
            e.Property(o => o.StripePaymentIntentId).HasMaxLength(256);
            e.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId);
            e.HasMany(o => o.Items).WithOne(i => i.Order!).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(o => o.UserId);
            e.HasIndex(o => o.StripeSessionId);
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.Property(i => i.UnitPrice).HasColumnType("numeric(12,2)");
            e.Property(i => i.ProductName).IsRequired().HasMaxLength(256);
            e.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
            e.Ignore(i => i.Subtotal);
        });

        modelBuilder.Entity<Cart>().Ignore(c => c.Total);
    }
}
