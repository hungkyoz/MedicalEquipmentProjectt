using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MedicalEquipmentProject.Repositories
{
    public class MedicalEquipmentRepository : IMedicalEquipmentRepository
    {
        private readonly AppDbContext _context;

        public MedicalEquipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<MedicalEquipment>> GetFilteredEquipmentAsync(MedicalEquipmentFilter filter)
        {
            var query = _context.MedicalEquipment
               .Include(e => e.AssignedUser) 
               .AsQueryable();


            if (!string.IsNullOrEmpty(filter.Model))
                query = query.Where(e => e.Model.Contains(filter.Model));

            if (filter.MinPrice.HasValue)
                query = query.Where(e => e.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(e => e.Price <= filter.MaxPrice.Value);

            return await PagedList<MedicalEquipment>.CreateAsync(query, filter.PageNumber, filter.PageSize);
        }
    }
}
