using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;

var builder = WebApplication.CreateBuilder(args);

// Load .env if present
try
{
    DotNetEnv.Env.Load();
}
catch { }

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure database connection
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // Fallback connection string for development
    connectionString = "Server=DESKTOP-5VDN8L9\\NQTAM;Database=BanVeXemPhim;Trusted_Connection=true;TrustServerCertificate=true;";
}

builder.Services.AddDbContext<Project2_Nhom5Context>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapGet("/", context =>
{
    context.Response.Redirect("/Admin");
    return Task.CompletedTask;
});

// Route cho khu vực Admin
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

// Route mặc định cho các controller ngoài khu vực
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();