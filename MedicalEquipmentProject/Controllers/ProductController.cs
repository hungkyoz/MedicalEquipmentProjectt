using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(new { errors = errorMessages });
            }

            var product = new Product
            {
                Name = model.Name,
                Quantity = model.Quantity,
                Date = model.Date,
                Price = model.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var image in model.Images)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var path = Path.Combine("wwwroot/product-images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = "/product-images/" + fileName
                    });
                }

                await _context.SaveChangesAsync();
            }

            var products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
            return PartialView("_ProductListPartial", products);
        }





    }
}
