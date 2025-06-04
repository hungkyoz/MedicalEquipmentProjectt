using System.ComponentModel.DataAnnotations;

namespace MedicalEquipmentProject.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required, EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
