using Publisher.Domain;
using Microsoft.EntityFrameworkCore;

namespace Publisher.Infra.Context;

public partial class MyDbContext : DbContext
{
    
    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<Products> Products { get; set; }
    
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Customername)
                .HasMaxLength(255)
                .HasColumnName("customername");
            entity.Property(e => e.Orderdate).HasColumnName("orderdate");
            entity.Property(e => e.Orderstatus)
                .HasMaxLength(50)
                .HasColumnName("orderstatus");

            entity.HasMany(d => d.Product).WithMany(p => p.Order)
                .UsingEntity<Dictionary<string, object>>(
                    "Orderproducts",
                    r => r.HasOne<Products>().WithMany()
                        .HasForeignKey("Productid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("orderproducts_productid_fkey"),
                    l => l.HasOne<Orders>().WithMany()
                        .HasForeignKey("Orderid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("orderproducts_orderid_fkey"),
                    j =>
                    {
                        j.HasKey("Orderid", "Productid").HasName("orderproducts_pkey");
                        j.ToTable("orderproducts");
                        j.IndexerProperty<Guid>("Orderid").HasColumnName("orderid");
                        j.IndexerProperty<Guid>("Productid").HasColumnName("productid");
                    });
        });

        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
