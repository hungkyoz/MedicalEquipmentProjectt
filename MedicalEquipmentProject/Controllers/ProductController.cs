using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.Services;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalEquipmentProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;


        private readonly CloudinaryService _cloudinaryService;

        public ProductController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList(int page = 1, int pageSize = 10)
        {
            var products = _context.Products
                                   .Include(p => p.ProductImages)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();
            return PartialView("_ProductListPartial", products);
        }

        private async Task<(string ImageUrl, string CloudUrl)> SaveProductImage(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + ".webp";
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "product-images");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, fileName);

            // Resize và lưu ra local
            using (var img = Image.Load(image.OpenReadStream()))
            {
                img.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(600, 0),
                    Mode = ResizeMode.Max
                }));

                await img.SaveAsync(filePath, new WebpEncoder());
            }

            // Dùng Stream lại để upload lên Cloudinary
            await using var stream = System.IO.File.OpenRead(filePath);
            var cloudUrl = await _cloudinaryService.UploadImageAsync(stream, fileName);

            return ($"/product-images/{fileName}", cloudUrl);
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
                    var (imageUrl, cloudUrl) = await SaveProductImage(image);

                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        CloudImageUrl = cloudUrl ?? "",
                        ImageUrl = imageUrl

                    });
                }

                await _context.SaveChangesAsync();
            }

            var products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
            return PartialView("_ProductListPartial", products);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            foreach (var image in product.ProductImages)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            var products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
            return PartialView("_ProductListPartial", products);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();
                return BadRequest(new { errors });
            }

            var product = await _context.Products.Include(p => p.ProductImages)
                                                 .FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product == null)
                return NotFound();

            product.Name = model.Name;
            product.Quantity = model.Quantity;
            product.Date = model.Date;
            product.Price = model.Price;

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var oldImage in product.ProductImages)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.ProductImages.RemoveRange(product.ProductImages);

                foreach (var image in model.Images)
                {
                    var (imageUrl, cloudUrl) = await SaveProductImage(image);

                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = imageUrl,
                        CloudImageUrl = cloudUrl ?? "",
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public IActionResult DownloadAllImages(int productId)
        {
            var product = _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == productId);

            if (product == null || product.ProductImages == null || !product.ProductImages.Any())
                return NotFound("Không có ảnh để tải.");

            var zipFileName = $"product_{productId}_images.zip";
            var zipPath = Path.Combine(Path.GetTempPath(), zipFileName);

            using (var zipStream = new FileStream(zipPath, FileMode.Create))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                foreach (var image in product.ProductImages)
                {
                    var fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullImagePath))
                    {
                        var entryName = Path.GetFileName(fullImagePath);
                        archive.CreateEntryFromFile(fullImagePath, entryName);
                    }
                }
            }

            var zipBytes = System.IO.File.ReadAllBytes(zipPath);
            System.IO.File.Delete(zipPath); // Xoá file tạm

            return File(zipBytes, "application/zip", zipFileName);
        }


    }
}