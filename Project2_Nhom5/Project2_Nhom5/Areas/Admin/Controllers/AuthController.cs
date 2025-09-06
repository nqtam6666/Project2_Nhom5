using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Project2_Nhom5.Services;
using System.Threading.Tasks;

namespace Project2_Nhom5.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var role = Request.Cookies["role"] ?? string.Empty;
            if (string.Equals(role, "Admin", System.StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/Admin" : returnUrl!);
            }

            ViewData["ReturnUrl"] = string.IsNullOrWhiteSpace(returnUrl) ? "/Admin" : returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            var user = await _authService.AuthenticateUserAsync(username, password);
            if (user == null || !string.Equals(user.Role, "Admin", System.StringComparison.OrdinalIgnoreCase))
            {
                ViewData["Error"] = "Sai tài khoản/mật khẩu hoặc không có quyền Admin.";
                ViewData["ReturnUrl"] = string.IsNullOrWhiteSpace(returnUrl) ? "/Admin" : returnUrl;
                return View();
            }

            Response.Cookies.Append("userId", user.UserId.ToString(), new CookieOptions { HttpOnly = true, IsEssential = true, MaxAge = System.TimeSpan.FromDays(30) });
            Response.Cookies.Append("username", user.Username, new CookieOptions { HttpOnly = true, IsEssential = true, MaxAge = System.TimeSpan.FromDays(30) });
            Response.Cookies.Append("role", "Admin", new CookieOptions { HttpOnly = true, IsEssential = true, MaxAge = System.TimeSpan.FromDays(30) });

            var dest = string.IsNullOrWhiteSpace(returnUrl) ? "/Admin" : returnUrl!;
            return Redirect(dest);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("userId");
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("role");
            return Redirect("/Admin/Auth/Login");
        }
    }
} 