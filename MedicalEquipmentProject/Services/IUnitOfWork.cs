namespace MedicalEquipmentProject.Services
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Rollback();
    }
}