using System.ComponentModel.DataAnnotations;

namespace MedicalEquipmentProject.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; } // Thêm trường Id để hỗ trợ chỉnh sửa sản phẩm

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày nhập")]
        [CustomValidation(typeof(ProductViewModel), nameof(ValidateDateNotInFuture))]
        public DateTime Date { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        public List<IFormFile> Images { get; set; }

        public static ValidationResult ValidateDateNotInFuture(DateTime date, ValidationContext context)
        {
            return date > DateTime.Today
                ? new ValidationResult("Ngày không được vượt quá hôm nay")
                : ValidationResult.Success;
        }
    }
}
