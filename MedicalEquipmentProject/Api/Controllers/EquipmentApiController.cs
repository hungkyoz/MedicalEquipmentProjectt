using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MedicalEquipmentProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EquipmentApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EquipmentApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.MedicalEquipment.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.MedicalEquipment.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MedicalEquipment equipment)
        {
            _context.MedicalEquipment.Add(equipment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = equipment.Id }, equipment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicalEquipment equipment)
        {
            if (id != equipment.Id)
                return BadRequest(new { message = "ID không khớp với dữ liệu gửi lên." });

            var existing = await _context.MedicalEquipment.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Không tìm thấy thiết bị cần cập nhật." });

            existing.Name = equipment.Name;
            existing.Model = equipment.Model;
            existing.Price = equipment.Price;
            existing.Manufacturer = equipment.Manufacturer;
            existing.PurchaseDate = equipment.PurchaseDate;
            existing.IsActive = equipment.IsActive;
            existing.AssignedUserId = equipment.AssignedUserId;

            _context.Entry(existing).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thiết bị thành công!", data = existing });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.MedicalEquipment.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Không tìm thấy thiết bị để xóa." });

            _context.MedicalEquipment.Remove(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thiết bị thành công!", deletedId = id });
        }

    }
}
