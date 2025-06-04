using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalEquipmentProject.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // Danh sách user theo role ( gom role user va role admin )
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("Admin"))
                return Forbid(); // Chặn user thường

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Profile()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null) return NotFound();

            var vm = new UserEditVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                CurrentAvatarPath = user.AvatarPath
            };

            return View("Edit", vm); 
        }


        //Edit: chỉ cho phép Admin hoặc chính chủ ( acc cua minh a )
        public async Task<IActionResult> Edit(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (!User.IsInRole("Admin") && id != currentUserId)
            {
                return Forbid(); // 403
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var vm = new UserEditVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                CurrentAvatarPath = user.AvatarPath
            };

            return View(vm);
        }

        // POST Edit
        // admin chinh sua het
        // user khong duoc chinh sua profile nguoi khac + ko dc edit MedicalEquipment + Delete
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditVM vm)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (!User.IsInRole("Admin") && vm.Id != currentUserId)
            {
                return Forbid(); // Ngăn user sửa người khác
            }

            if (!ModelState.IsValid)
                return View(vm);

            var user = await _context.Users.FindAsync(vm.Id);
            if (user == null) return NotFound();

            user.FullName = vm.FullName;
            user.Email = vm.Email;

            if (vm.Avatar != null)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.Avatar.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.Avatar.CopyToAsync(stream);
                }

                user.AvatarPath = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction("Index");
        }
    }
}
