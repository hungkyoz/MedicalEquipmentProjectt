using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MedicalEquipmentProject.Services
{
    public class MedicalEquipmentServiceWithUoW
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public MedicalEquipmentServiceWithUoW(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CreateWithImagesAsync(MedicalEquipment equipment, List<IFormFile> images)
        {
            try
            {
                _context.MedicalEquipment.Add(equipment);

                foreach (var image in images)
                {
                    var ext = Path.GetExtension(image.FileName).ToLower();
                    var mime = image.ContentType.ToLower();

                    // Kiểm tra định dạng đuôi file
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                        throw new Exception($"File '{image.FileName}' không có đuôi hợp lệ (.jpg/.png)");

                    // Kiểm tra MIME type
                    if (!mime.StartsWith("image/"))
                        throw new Exception($"File '{image.FileName}' không phải ảnh hợp lệ (MIME type: {mime})");

                    // Kiểm tra kích thước (giới hạn 5MB)
                    if (image.Length > 5 * 1024 * 1024)
                        throw new Exception($"File '{image.FileName}' vượt quá 5MB");

                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/equipment-images");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = image.OpenReadStream())
                    using (var img = Image.Load(stream))
                    {
                        img.Mutate(x => x.Resize(1091, 768));
                        await img.SaveAsync(filePath);
                    }

                    _context.MedicalEquipmentImages.Add(new MedicalEquipmentImage
                    {
                        Equipment = equipment,
                        ImagePath = "/equipment-images/" + fileName
                    });
                }

                await _unitOfWork.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return (false, ex.Message);
            }
        }

        public async Task<List<MedicalEquipment>> GetFilteredEquipmentAsync(MedicalEquipmentFilter filter)
        {
            var query = _context.MedicalEquipment
                .Include(e => e.Images)
                .Include(e => e.AssignedUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(e => e.Name.Contains(filter.Name));

            if (filter.IsActive.HasValue)
                query = query.Where(e => e.IsActive == filter.IsActive);

            if (filter.AssignedUserId.HasValue)
                query = query.Where(e => e.AssignedUserId == filter.AssignedUserId);

            return await query.ToListAsync();
        }
    }
}
