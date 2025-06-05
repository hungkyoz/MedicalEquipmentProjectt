using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.Services;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MedicalEquipmentProject.Controllers
{
    [Authorize]
    public class MedicalEquipmentController : Controller
    {
        private readonly IMedicalEquipmentService _service;
        private readonly AppDbContext _context;

        public MedicalEquipmentController(IMedicalEquipmentService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task<IActionResult> Index(MedicalEquipmentFilter filter)
        {
            var result = await _service.GetFilteredEquipmentAsync(filter);
            ViewBag.Filter = filter;
            return View(result);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.MedicalEquipment
                .Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (item == null) return NotFound();

            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");

            return View(new MedicalEquipmentEditVM
            {
                Id = item.Id,
                Name = item.Name,
                Model = item.Model,
                Price = item.Price,
                Manufacturer = item.Manufacturer,
                PurchaseDate = item.PurchaseDate,
                IsActive = item.IsActive,
                AssignedUserId = item.AssignedUserId,
                ExistingImagePaths = item.Images?.Select(i => i.ImagePath).ToList()
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(MedicalEquipmentEditVM vm)
        {
            if (vm.PurchaseDate > DateTime.Today)
            {
                TempData["ErrorMessage"] = "Purchase date cannot be in the future!";
                ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");
                return View(vm);
            }

            var item = await _context.MedicalEquipment.FindAsync(vm.Id);
            if (item == null) return NotFound();

            item.Name = vm.Name;
            item.Model = vm.Model;
            item.Price = vm.Price;
            item.Manufacturer = vm.Manufacturer;
            item.PurchaseDate = vm.PurchaseDate;
            item.IsActive = vm.IsActive;
            item.AssignedUserId = vm.AssignedUserId;

            // ✅ Upload nhiều ảnh thiết bị & tự động resize về 1091x768 px
            if (vm.Images != null && vm.Images.Count > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/equipment-images");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                foreach (var image in vm.Images)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = image.OpenReadStream())
                    using (var img = SixLabors.ImageSharp.Image.Load(stream))
                    {
                        img.Mutate(x => x.Resize(1091, 768));
                        await img.SaveAsync(filePath);
                    }

                    var equipmentImage = new MedicalEquipmentImage
                    {
                        EquipmentId = item.Id,
                        ImagePath = "/equipment-images/" + fileName
                    };

                    _context.Set<MedicalEquipmentImage>().Add(equipmentImage);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Device has been updated successfully";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var equipment = await _context.MedicalEquipment
                                          .Include(e => e.Images)
                                          .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
                return NotFound();

            // ✅ Xóa tất cả ảnh liên quan trước
            if (equipment.Images != null && equipment.Images.Any())
            {
                _context.Set<MedicalEquipmentImage>().RemoveRange(equipment.Images);

            }

            _context.MedicalEquipment.Remove(equipment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Device remove succesfull!";
            return RedirectToAction("Index");
        }

    }
}
