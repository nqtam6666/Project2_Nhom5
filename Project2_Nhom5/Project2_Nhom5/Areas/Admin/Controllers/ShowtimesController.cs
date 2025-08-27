using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;

namespace Project2_Nhom5.Controllers
{
    [Area("Admin")]
    public class ShowtimesController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public ShowtimesController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Showtimes
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var project2_Nhom5Context = _context.Showtimes.Include(s => s.Movie).Include(s => s.Theater);
            return View(await project2_Nhom5Context.ToListAsync());
        }

        // GET: Showtimes/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(m => m.ShowtimeId == id);
            if (showtime == null)
            {
                return NotFound();
            }

            return View(showtime);
        }

        // GET: Showtimes/Create
        [HttpGet]
        public IActionResult Create()
        {
            var movies = _context.Movies.ToList();
            var theaters = _context.Theaters.ToList();
            
            var movieList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Chọn phim...", Selected = true }
            };
            movieList.AddRange(movies.Select(m => new SelectListItem { Value = m.MovieId.ToString(), Text = m.Title }));
            
            var theaterList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Chọn rạp chiếu...", Selected = true }
            };
            theaterList.AddRange(theaters.Select(t => new SelectListItem { Value = t.TheaterId.ToString(), Text = t.Name }));
            
            ViewBag.MovieId = new SelectList(movieList, "Value", "Text");
            ViewBag.TheaterId = new SelectList(theaterList, "Value", "Text");
            return View();
        }

        // POST: Showtimes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] int MovieId, [FromForm] int TheaterId, [FromForm] DateTime ShowDate, [FromForm] TimeSpan ShowTime)
        {
            // Create new showtime object
            var showtime = new Showtime
            {
                MovieId = MovieId,
                TheaterId = TheaterId,
                ShowDate = DateOnly.FromDateTime(ShowDate),
                ShowTime = TimeOnly.FromTimeSpan(ShowTime)
            };

            // Validate that MovieId and TheaterId exist
            var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == MovieId);
            var theaterExists = await _context.Theaters.AnyAsync(t => t.TheaterId == TheaterId);

            if (!movieExists)
            {
                ModelState.AddModelError("MovieId", "Phim được chọn không tồn tại trong hệ thống.");
            }

            if (!theaterExists)
            {
                ModelState.AddModelError("TheaterId", "Rạp chiếu được chọn không tồn tại trong hệ thống.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.MovieId = new SelectList(_context.Movies, "MovieId", "Title", MovieId);
            ViewBag.TheaterId = new SelectList(_context.Theaters, "TheaterId", "Name", TheaterId);
            return View(showtime);
        }

        // GET: Showtimes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch chiếu yêu cầu.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MovieId = new SelectList(_context.Movies, "MovieId", "Title", showtime.MovieId);
            ViewBag.TheaterId = new SelectList(_context.Theaters, "TheaterId", "Name", showtime.TheaterId);
            return View(showtime);
        }

        // POST: Showtimes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] int ShowtimeId, [FromForm] int MovieId, [FromForm] int TheaterId, [FromForm] DateTime ShowDate, [FromForm] TimeSpan ShowTime)
        {
            if (id != ShowtimeId)
            {
                return NotFound();
            }

            // Create showtime object
            var showtime = new Showtime
            {
                ShowtimeId = ShowtimeId,
                MovieId = MovieId,
                TheaterId = TheaterId,
                ShowDate = DateOnly.FromDateTime(ShowDate),
                ShowTime = TimeOnly.FromTimeSpan(ShowTime)
            };

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(showtime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowtimeExists(showtime.ShowtimeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.MovieId = new SelectList(_context.Movies, "MovieId", "Title", MovieId);
            ViewBag.TheaterId = new SelectList(_context.Theaters, "TheaterId", "Name", TheaterId);
            return View(showtime);
        }

        // GET: Showtimes/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(m => m.ShowtimeId == id);
            if (showtime == null)
            {
                return NotFound();
            }

            return View(showtime);
        }

        // POST: Showtimes/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime != null)
            {
                _context.Showtimes.Remove(showtime);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Debug action to check available data
        [HttpGet]
        public async Task<IActionResult> DebugData()
        {
            var movies = await _context.Movies.ToListAsync();
            var theaters = await _context.Theaters.ToListAsync();
            
            var result = $"Movies ({movies.Count}):\n";
            foreach (var movie in movies)
            {
                result += $"  MovieId: {movie.MovieId}, Title: {movie.Title}\n";
            }
            
            result += $"\nTheaters ({theaters.Count}):\n";
            foreach (var theater in theaters)
            {
                result += $"  TheaterId: {theater.TheaterId}, Name: {theater.Name}\n";
            }
            
            return Content(result);
        }

        private bool ShowtimeExists(int id)
        {
            return _context.Showtimes.Any(e => e.ShowtimeId == id);
        }
    }
}
