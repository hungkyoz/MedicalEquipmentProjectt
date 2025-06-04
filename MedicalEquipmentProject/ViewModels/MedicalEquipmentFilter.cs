using System.ComponentModel.DataAnnotations;

namespace MedicalEquipmentProject.ViewModels
{
    public class MedicalEquipmentFilter
    {
        public string Model { get; set; }
        [Range(0, 100000000)]
        public decimal? MinPrice { get; set; }
        [Range(0, 100000000)]
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50; // Feature #1

        public bool IsEmpty() =>
        string.IsNullOrEmpty(Model) &&
        !MinPrice.HasValue &&
        !MaxPrice.HasValue;

    }
}