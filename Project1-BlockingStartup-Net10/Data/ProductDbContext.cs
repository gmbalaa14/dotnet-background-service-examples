using Microsoft.EntityFrameworkCore;
using DotNet10.NonBlockingStartup.Api.Models;

namespace DotNet10.NonBlockingStartup.Api.Data;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("datetime('now')");
        });

        // Add indexes for better performance
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Category)
            .HasDatabaseName("IX_Products_Category");

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Price)
            .HasDatabaseName("IX_Products_Price");
    }
}
