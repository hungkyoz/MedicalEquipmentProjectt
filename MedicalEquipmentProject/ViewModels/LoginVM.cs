using System.ComponentModel.DataAnnotations;

namespace MedicalEquipmentProject.ViewModels
{
    public class LoginVM
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}