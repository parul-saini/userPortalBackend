using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace userPortalBackend.presentation.Data.Models;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AddressPortal> AddressPortals { get; set; }

    public virtual DbSet<UserPortal> UserPortals { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddressPortal>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__addressP__26A1118D731D5FC4");

            entity.ToTable("addressPortal");

            entity.Property(e => e.AddressId).HasColumnName("addressID");
            entity.Property(e => e.AddressLine1).HasMaxLength(255);
            entity.Property(e => e.AddressLine2).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.City2).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Country2).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.State2).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.ZipCode).HasMaxLength(10);
            entity.Property(e => e.ZipCode2).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.AddressPortals)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__addressPo__userI__1C880743");
        });

        modelBuilder.Entity<UserPortal>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__userPort__CB9A1CDFFD97B610");

            entity.ToTable("userPortal");

            entity.HasIndex(e => e.Email, "UQ__userPort__A9D10534ED0A6408").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.AlternatePhone).HasMaxLength(15);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(1000)
                .HasColumnName("ImageURL");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__userPorta__Creat__19AB9A98");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
