using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalEquipmentProject.ViewModels
{
    public class MedicalEquipmentEditVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Model { get; set; }

        [Range(0, 100000000, ErrorMessage = "Price must be non-negative")]
        public decimal Price { get; set; }

        public string Manufacturer { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Purchase Date")]
        [CustomValidation(typeof(MedicalEquipmentEditVM), nameof(ValidatePurchaseDate))]
        public DateTime PurchaseDate { get; set; }

        public bool IsActive { get; set; }

        public int? AssignedUserId { get; set; }

        public List<IFormFile>? Images { get; set; } // Nhiều ảnh upload
        public List<string>? ExistingImagePaths { get; set; } // Hiển thị lại ảnh cũ (nếu cần)

        public static ValidationResult ValidatePurchaseDate(DateTime purchaseDate, ValidationContext context)
        {
            if (purchaseDate > DateTime.Today)
                return new ValidationResult("Purchase Date cannot be in the future.");
            return ValidationResult.Success;
        }
    }
}
