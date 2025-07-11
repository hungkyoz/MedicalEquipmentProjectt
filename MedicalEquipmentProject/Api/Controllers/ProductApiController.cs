using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Models;
using MedicalEquipmentProject.Services;
using MedicalEquipmentProject.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace MedicalEquipmentProject.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]

    public class ProductApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public ProductApiController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromForm] ProductViewModel model)
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

            var uploadedImages = new List<string>();

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

                    uploadedImages.Add(cloudUrl ?? "");
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Thêm sản phẩm thành công",
                productId = product.Id,
                cloudImageLinks = uploadedImages
            });
        }

        private async Task<(string ImageUrl, string CloudUrl)> SaveProductImage(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + ".webp";
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "product-images");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, fileName);

            using (var img = Image.Load(image.OpenReadStream()))
            {
                img.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(600, 0),
                    Mode = ResizeMode.Max
                }));

                await img.SaveAsync(filePath, new WebpEncoder());
            }

            await using var stream = System.IO.File.OpenRead(filePath);
            var cloudUrl = await _cloudinaryService.UploadImageAsync(stream, fileName);

            return ($"/product-images/{fileName}", cloudUrl);
        }
    }

}