using System.ComponentModel.DataAnnotations;

namespace MedicalEquipmentProject.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        public string? AvatarPath { get; set; }

        public string? Bio { get; set; }

        public string Role { get; set; } = "User";

        

        // Quan hệ 1-nhiều: 1 User có thể được cấp nhiều thiết bị y tế
        public ICollection<MedicalEquipment>? AllocatedEquipments { get; set; }
    }
}
