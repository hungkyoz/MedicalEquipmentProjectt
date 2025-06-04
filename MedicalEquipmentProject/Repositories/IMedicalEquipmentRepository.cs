using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.ViewModels;
using System.Threading.Tasks;

namespace MedicalEquipmentProject.Repositories
{
    public interface IMedicalEquipmentRepository
    {
        Task<PagedList<MedicalEquipment>> GetFilteredEquipmentAsync(MedicalEquipmentFilter filter);
    }
}
