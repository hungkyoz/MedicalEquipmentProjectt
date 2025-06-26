using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace MedicalEquipmentProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // Index page to display the list of products
        public IActionResult Index()
        {
            return View();
        }

        // Paginated list of products
        public IActionResult GetList(int page = 1, int pageSize = 10)
        {
            var products = _context.Products
                                   .Include(p => p.ProductImages)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();
            return PartialView("_ProductListPartial", products);
        }

        // Helper method to save product images
        private async Task<string> SaveProductImage(IFormFile image)
        {
            try
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "product-images");

                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return "/product-images/" + fileName;
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading image: " + ex.Message);
            }
        }

        // Add product (Admin only)
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
                    var imageUrl = await SaveProductImage(image); // Call helper method
                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = imageUrl
                    });
                }

                await _context.SaveChangesAsync();
            }

            var products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
            return PartialView("_ProductListPartial", products);
        }

        // Delete product (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Delete images
            foreach (var image in product.ProductImages)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Delete product
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            var products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
            return PartialView("_ProductListPartial", products);
        }

        // Edit product (Admin only) - GET method to display the form
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
                Date = product.Date,
                Price = product.Price
            };

            return View(model);
        }

        // Edit product (Admin only) - POST method to save changes
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = model.Name;
            product.Quantity = model.Quantity;
            product.Date = model.Date;
            product.Price = model.Price;

            // Delete old images if new ones are uploaded
            if (model.Images != null && model.Images.Count > 0)
            {
                // Delete old images from directory
                foreach (var oldImage in product.ProductImages)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Delete old images from database
                _context.ProductImages.RemoveRange(product.ProductImages);

                // Add new images
                foreach (var image in model.Images)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine("wwwroot", "product-images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = "/product-images/" + fileName
                    });
                }
            }

            await _context.SaveChangesAsync();

            var products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
            return PartialView("_ProductListPartial", products);
        }
    }
}
