using Microsoft.EntityFrameworkCore;
using MedicalEquipmentProject.Models;

namespace MedicalEquipmentProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MedicalEquipment> MedicalEquipment { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicalEquipment>().ToTable("MedicalEquipment");
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}
