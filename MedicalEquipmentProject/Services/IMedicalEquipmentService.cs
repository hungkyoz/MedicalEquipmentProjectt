using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.ViewModels;

namespace MedicalEquipmentProject.Services
{
    public interface IMedicalEquipmentService
    {
        Task<PagedList<MedicalEquipment>> GetFilteredEquipmentAsync(MedicalEquipmentFilter filter);


    }
}