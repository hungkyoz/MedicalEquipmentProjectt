using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.Repositories;
using MedicalEquipmentProject.ViewModels;

namespace MedicalEquipmentProject.Services
{
    public class MedicalEquipmentService : IMedicalEquipmentService
    {
        private readonly IMedicalEquipmentRepository _repository;

        public MedicalEquipmentService(IMedicalEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedList<MedicalEquipment>> GetFilteredEquipmentAsync(MedicalEquipmentFilter filter)
        {
            return await _repository.GetFilteredEquipmentAsync(filter);
        }
    }
}