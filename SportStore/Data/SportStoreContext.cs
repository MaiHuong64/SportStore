using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SportStore.Models;

namespace SportStore.Data;

public partial class SportStoreContext : DbContext
{
    public SportStoreContext(DbContextOptions<SportStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.ProductDetailId).HasName("PK__ProductD__3C8DD694E667FEB0");

            entity.ToTable("ProductDetail");

            entity.HasIndex(e => e.Sku, "UQ__ProductD__CA1ECF0D3AA42D2D").IsUnique();

            entity.Property(e => e.ProductDetailId).HasColumnName("ProductDetailID");
            entity.Property(e => e.Color).HasMaxLength(30);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
