using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;

namespace Project2_Nhom5.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class MoviesController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public MoviesController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.MovieId)
                .ToListAsync();

            ViewData["IsLoggedIn"] = !string.IsNullOrEmpty(Request.Cookies["userId"]);
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(movies);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            ViewData["IsLoggedIn"] = !string.IsNullOrEmpty(Request.Cookies["userId"]);
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(movie);
        }
    }
} 