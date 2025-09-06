using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Project2_Nhom5.Controllers;

public class HomeController : Controller
{
    // Trang khách (/) — landing page
    public IActionResult Index()
    {
        // Đọc role từ cookie demo (nếu có)
        var role = Request.Cookies["role"] ?? string.Empty;
        ViewData["IsAdmin"] = string.Equals(role, "Admin", System.StringComparison.OrdinalIgnoreCase);
        return View();
    }

    // Demo: Đăng nhập admin (đặt cookie role=Admin)
    [HttpPost]
    public IActionResult LoginAsAdmin()
    {
        Response.Cookies.Append("role", "Admin", new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            MaxAge = System.TimeSpan.FromHours(8)
        });
        return RedirectToAction("Index");
    }

    // Demo: Đăng xuất (xóa cookie role)
    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("role");
        return RedirectToAction("Index");
    }
} 