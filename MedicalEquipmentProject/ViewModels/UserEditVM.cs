using System.ComponentModel.DataAnnotations;

namespace MedicalEquipmentProject.ViewModels
{
    public class UserEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public IFormFile? Avatar { get; set; }
        [StringLength(500)]
        public string? Bio { get; set; }

        public string? CurrentAvatarPath { get; set; } // Hiển thị ảnh hiện tại
    }
}
