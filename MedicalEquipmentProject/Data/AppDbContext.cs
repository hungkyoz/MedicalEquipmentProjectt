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

                // Seed data
                entity.HasData(
                    new Product
                    {
                        Id = 1,
                        Name = "Bàn làm việc",
                        Quantity = 10,
                        Date = DateTime.Today.AddDays(-5),
                        Price = 2500000
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Ghế văn phòng",
                        Quantity = 15,
                        Date = DateTime.Today.AddDays(-10),
                        Price = 1500000
                    }
                );
            });

            // Configure ProductImages
            modelBuilder.Entity<ProductImage>()
                .HasOne(p => p.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed ProductImages data
            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage
                {
                    Id = 1,
                    ProductId = 1,
                    ImageUrl = "/product-images/1.sm.webp"
                },
                new ProductImage
                {
                    Id = 2,
                    ProductId = 2,
                    ImageUrl = "/product-images/2.webp"
                }
            );
        }
    }
}