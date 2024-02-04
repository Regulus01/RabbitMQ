using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Publisher.Domain;

namespace Publisher.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Payments> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payments>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.Paymentid)
                .ValueGeneratedNever()
                .HasColumnName("paymentid");
            entity.Property(e => e.Paymentamount)
                .HasPrecision(10, 2)
                .HasColumnName("paymentamount");
            entity.Property(e => e.Paymentdate).HasColumnName("paymentdate");
            entity.Property(e => e.Paymentstatus)
                .HasMaxLength(50)
                .HasColumnName("paymentstatus");
            entity.Property(e => e.Relatedid).HasColumnName("relatedid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
