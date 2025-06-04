using MedicalEquipmentProject.Data;
using MedicalEquipmentProject.Repositories; // Ensure this namespace is included
using MedicalEquipmentProject.Services; // Add this namespace to resolve IMedicalEquipmentService and MedicalEquipmentService
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBString")));

builder.Services.AddScoped<IMedicalEquipmentRepository, MedicalEquipmentRepository>();
builder.Services.AddScoped<IMedicalEquipmentService, MedicalEquipmentService>();



builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MedicalEquipment}/{action=Index}/{id?}");


app.Run();
