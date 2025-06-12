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
        private readonly IMedicalEquipmentService _readService;

        private readonly MedicalEquipmentServiceWithUoW _writeService;
        private readonly AppDbContext _context;

        public MedicalEquipmentController(
            IMedicalEquipmentService readService,
            MedicalEquipmentServiceWithUoW writeService,
            AppDbContext context)
        {
            _readService = readService;
            _writeService = writeService;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(MedicalEquipmentFilter filter)
        {
            var result = await _readService.GetFilteredEquipmentAsync(filter);
            ViewBag.Filter = filter;
            return View(result);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MedicalEquipment equipment, List<IFormFile> Images)
        {
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");

            if (equipment.PurchaseDate > DateTime.Today)
            {
                TempData["ErrorMessage"] = "Ngày mua không được lớn hơn hôm nay.";
                return View(equipment);
            }

            var (success, errorMessage) = await _writeService.CreateWithImagesAsync(equipment, Images);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage;
                return View(equipment);
            }

            TempData["SuccessMessage"] = "Device have to add new Succesfull";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.MedicalEquipment.Include(e => e.Images).FirstOrDefaultAsync(e => e.Id == id);
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

            if (vm.Images != null && vm.Images.Count > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/equipment-images");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                foreach (var image in vm.Images)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = image.OpenReadStream())
                    using (var img = Image.Load(stream))
                    {
                        img.Mutate(x => x.Resize(1091, 768));
                        await img.SaveAsync(filePath);
                    }

                    _context.Set<MedicalEquipmentImage>().Add(new MedicalEquipmentImage
                    {
                        EquipmentId = item.Id,
                        ImagePath = "/equipment-images/" + fileName
                    });
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
            var equipment = await _context.MedicalEquipment.Include(e => e.Images).FirstOrDefaultAsync(e => e.Id == id);
            if (equipment == null) return NotFound();

            if (equipment.Images != null && equipment.Images.Any())
            {
                _context.Set<MedicalEquipmentImage>().RemoveRange(equipment.Images);
            }

            _context.MedicalEquipment.Remove(equipment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Device removed successfully!";
            return RedirectToAction("Index");
        }
    }
}
