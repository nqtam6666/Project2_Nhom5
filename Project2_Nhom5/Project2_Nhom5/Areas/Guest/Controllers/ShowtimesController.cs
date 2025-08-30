using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;

namespace Project2_Nhom5.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class ShowtimesController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public ShowtimesController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);

            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .Where(s => s.ShowDate > today || 
                           (s.ShowDate == today && s.ShowTime > now))
                .OrderBy(s => s.ShowDate)
                .ThenBy(s => s.ShowTime)
                .ToListAsync();

            ViewData["IsLoggedIn"] = !string.IsNullOrEmpty(Request.Cookies["userId"]);
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(showtimes);
        }
    }
} 