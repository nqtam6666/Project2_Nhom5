using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;

namespace Project2_Nhom5.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class TheatersController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public TheatersController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var theaters = await _context.Theaters
                .OrderBy(t => t.TheaterId)
                .ToListAsync();

            ViewData["IsLoggedIn"] = !string.IsNullOrEmpty(Request.Cookies["userId"]);
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(theaters);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theater = await _context.Theaters
                .FirstOrDefaultAsync(t => t.TheaterId == id);

            if (theater == null)
            {
                return NotFound();
            }

            ViewData["IsLoggedIn"] = !string.IsNullOrEmpty(Request.Cookies["userId"]);
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(theater);
        }
    }
} 