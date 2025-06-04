using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.Services;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index(MedicalEquipmentProject.ViewModels.MedicalEquipmentFilter filter)
        {
            var result = await _service.GetFilteredEquipmentAsync(filter);
            ViewBag.Filter = filter;
            return View(result);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.MedicalEquipment.FindAsync(id);
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
                AssignedUserId = item.AssignedUserId
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

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thiết bị đã được cập nhật thành công!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var equipment = await _context.MedicalEquipment.FindAsync(id);
            if (equipment == null)
                return NotFound();

            _context.MedicalEquipment.Remove(equipment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Remove succesfull.";
            return RedirectToAction("Index");
        }



    }
}
