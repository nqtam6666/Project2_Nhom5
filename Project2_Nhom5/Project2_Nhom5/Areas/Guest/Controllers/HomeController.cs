using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;
using Project2_Nhom5.Areas.Guest.Models;

namespace Project2_Nhom5.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public HomeController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = Request.Cookies["role"] ?? string.Empty;
            ViewData["IsAdmin"] = string.Equals(role, "Admin", System.StringComparison.OrdinalIgnoreCase);

            var vm = new GuestHomeViewModel
            {
                NowShowing = await _context.Movies.Where(m => m.Status == "dangchieu").OrderByDescending(m => m.MovieId).Take(6).ToListAsync(),
                ComingSoon = await _context.Movies.Where(m => m.Status == "sapchieu").OrderByDescending(m => m.MovieId).Take(6).ToListAsync(),
                Stopped = await _context.Movies.Where(m => m.Status == "ngungchieu").OrderByDescending(m => m.MovieId).Take(6).ToListAsync()
            };
            return View(vm);
        }

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

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("role");
            return RedirectToAction("Index");
        }
    }
} 