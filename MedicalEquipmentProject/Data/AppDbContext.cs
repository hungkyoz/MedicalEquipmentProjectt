using Microsoft.EntityFrameworkCore;
using MedicalEquipmentProject.Models;

namespace MedicalEquipmentProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MedicalEquipment> MedicalEquipment { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MedicalEquipmentImage> MedicalEquipmentImages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicalEquipment>().ToTable("MedicalEquipment");
            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<MedicalEquipmentImage>()
              .ToTable("MedicalEquipmentImages")
              .HasOne(img => img.Equipment)
              .WithMany(eq => eq.Images)
              .HasForeignKey(img => img.EquipmentId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
