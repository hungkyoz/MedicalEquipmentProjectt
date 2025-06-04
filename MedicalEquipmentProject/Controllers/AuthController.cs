using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalEquipmentProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == vm.Email && u.Password == vm.Password);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid credentials";
                return View(vm);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("AvatarPath", user.AvatarPath ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "MedicalEquipment");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

 \
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Kiểm tra email đã tồn tại chưa
            if (_context.Users.Any(u => u.Email == vm.Email))
            {
                TempData["ErrorMessage"] = "Email đã được sử dụng.";
                return View(vm);
            }

            var newUser = new User
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Password = vm.Password, 
                Role = "User" 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Login Succesfull , Welcome User";
            return RedirectToAction("Login");
        }
    }
}
