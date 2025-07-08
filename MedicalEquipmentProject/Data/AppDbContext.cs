using MedicalEquipmentProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalEquipmentProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MedicalEquipment> MedicalEquipment { get; set; }
        public DbSet<MedicalEquipmentImage> MedicalEquipmentImages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure MedicalEquipment
            modelBuilder.Entity<MedicalEquipment>(entity =>
            {
                entity.ToTable("MedicalEquipment");
                entity.Property(e => e.Price)
                    .HasPrecision(18, 2); // Fix decimal precision warning
            });

            // Configure Users (assuming table already exists)
            modelBuilder.Entity<User>().ToTable("Users");

            // Configure MedicalEquipmentImages
            modelBuilder.Entity<MedicalEquipmentImage>()
                .ToTable("MedicalEquipmentImages")
                .HasOne(img => img.Equipment)
                .WithMany(eq => eq.Images)
                .HasForeignKey(img => img.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                    .HasPrecision(18, 2); // Fix decimal precision warning

                
            });

            // Configure ProductImages
            modelBuilder.Entity<ProductImage>()
                .HasOne(p => p.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}