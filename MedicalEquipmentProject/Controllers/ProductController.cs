using MedicalEquipmentProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalEquipmentProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList()
        {
            var products = _context.Products.Include(p => p.ProductImages).ToList();
            return PartialView("_ProductListPartial", products);
        }
    }
}
