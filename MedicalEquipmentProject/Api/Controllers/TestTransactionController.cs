using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalEquipmentProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestTransactionController : Controller
    {
        private readonly MedicalEquipmentServiceWithUoW _service;

        public TestTransactionController(MedicalEquipmentServiceWithUoW service)
        {
            _service = service;
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(MedicalEquipment equipment, List<IFormFile> Images)
        {
            var (success, errorMessage) = await _service.CreateWithImagesAsync(equipment, Images);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage;
                return View("Create");
            }
            else
            {
                TempData["SuccessMessage"] = "Thiết bị đã được thêm thành công!";
                return RedirectToAction("Index", "MedicalEquipment");
            }
        }
    }
}