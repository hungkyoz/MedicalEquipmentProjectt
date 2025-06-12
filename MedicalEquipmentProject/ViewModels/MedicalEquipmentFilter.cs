using System.ComponentModel.DataAnnotations;

namespace MedicalEquipmentProject.ViewModels
{
    public class MedicalEquipmentFilter
    {
        public string? Name { get; set; }           
        public string? Model { get; set; }          
        [Range(0, 100000000)]
        public decimal? MinPrice { get; set; }

        [Range(0, 100000000)]
        public decimal? MaxPrice { get; set; }

        public bool? IsActive { get; set; }         

        public int? AssignedUserId { get; set; }   

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        public bool IsEmpty() =>
            string.IsNullOrEmpty(Name) &&
            string.IsNullOrEmpty(Model) &&
            !MinPrice.HasValue &&
            !MaxPrice.HasValue &&
            !IsActive.HasValue &&
            !AssignedUserId.HasValue;
    }
}
