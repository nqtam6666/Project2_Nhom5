using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;
using Project2_Nhom5.Services;
using Project2_Nhom5.ModelBinders;

var builder = WebApplication.CreateBuilder(args);

// Load .env if present
try
{
    DotNetEnv.Env.Load();
}
catch { }

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Configure model binding for DateOnly and TimeOnly
    options.ModelBinderProviders.Insert(0, new DateOnlyTimeOnlyModelBinderProvider());
});

// Configure JSON options for DateOnly and TimeOnly
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(new Project2_Nhom5.ModelBinders.DateOnlyJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new Project2_Nhom5.ModelBinders.TimeOnlyJsonConverter());
});

// Add JSON converters for DateOnly and TimeOnly
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new Project2_Nhom5.ModelBinders.DateOnlyJsonConverter());
    options.SerializerOptions.Converters.Add(new Project2_Nhom5.ModelBinders.TimeOnlyJsonConverter());
});

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

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();

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

// Create a guest user in DB for anonymous visitors and set cookies
app.Use(async (context, next) =>
{
    var userId = context.Request.Cookies["userId"];
    var username = context.Request.Cookies["username"];
    var role = context.Request.Cookies["role"];

    // Only auto-create guest for GET requests and skip auth endpoints to avoid interfering with login/logout/register
    var path = context.Request.Path.Value ?? string.Empty;
    var pathLower = path.ToLowerInvariant();
    var isAuthEndpoint = pathLower.Contains("/guest/home/login") || pathLower.Contains("/guest/home/logout") || pathLower.Contains("/guest/home/register");
    var isGet = string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase);

    if (isGet || isAuthEndpoint || pathLower.StartsWith("/admin"))
    {
        await next();
        return;
    }

    await next();
});

// Simple Admin area gate using role cookie
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
    if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
    {
        var lower = path.ToLowerInvariant();
        var isAdminAuth = lower.StartsWith("/admin/auth/login") || lower.StartsWith("/admin/auth/logout");
        var role = context.Request.Cookies["role"] ?? string.Empty;
        var isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && !isAdminAuth)
        {
            context.Response.Redirect("/Admin/Auth/Login");
            return;
        }
    }
    await next();
});

app.UseAuthorization();


// Route cho khu vực Admin
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

// Route cho khu vực Guest (root)
app.MapAreaControllerRoute(
    name: "guest_root",
    areaName: "Guest",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Route mặc định (fallback)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Guest" });

app.Run();