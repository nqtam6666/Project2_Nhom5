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
    public class RevenuesController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public RevenuesController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Revenues
        public async Task<IActionResult> Index()
        {
            var project2_Nhom5Context = _context.Revenues
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Theater)
                .OrderByDescending(r => r.RevenueId);
            return View(await project2_Nhom5Context.ToListAsync());
        }

        // GET: Revenues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var revenue = await _context.Revenues
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Theater)
                .FirstOrDefaultAsync(m => m.RevenueId == id);
            if (revenue == null)
            {
                return NotFound();
            }

            return View(revenue);
        }

        // GET: Revenues/Create
        public IActionResult Create()
        {
            // Hiển thị tất cả lịch chiếu
            var availableShowtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .AsEnumerable()
                .Select(s => new { s.ShowtimeId, DisplayText = $"#{s.ShowtimeId} - {s.Movie?.Title ?? "N/A"} ({s.ShowDate:dd/MM/yyyy} {s.ShowTime:HH:mm}) - {s.Theater?.Name ?? "N/A"}" })
                .ToList();

            ViewData["ShowtimeId"] = new SelectList(availableShowtimes, "ShowtimeId", "DisplayText");
            return View();
        }

        // POST: Revenues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RevenueId,ShowtimeId,TotalAmount,AgencyCommission")] Revenue revenue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(revenue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown data
            var availableShowtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .AsEnumerable()
                .Select(s => new { s.ShowtimeId, DisplayText = $"#{s.ShowtimeId} - {s.Movie?.Title ?? "N/A"} ({s.ShowDate:dd/MM/yyyy} {s.ShowTime:HH:mm}) - {s.Theater?.Name ?? "N/A"}" })
                .ToList();

            ViewData["ShowtimeId"] = new SelectList(availableShowtimes, "ShowtimeId", "DisplayText", revenue.ShowtimeId);
            return View(revenue);
        }

        // GET: Revenues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var revenue = await _context.Revenues
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Theater)
                .FirstOrDefaultAsync(r => r.RevenueId == id);
            
            if (revenue == null)
            {
                return NotFound();
            }

            // Hiển thị thông tin lịch chiếu hiện tại
            var showtimeInfo = $"#{revenue.ShowtimeId} - {revenue.Showtime?.Movie?.Title ?? "N/A"} ({revenue.Showtime?.ShowDate:dd/MM/yyyy} {revenue.Showtime?.ShowTime:HH:mm}) - {revenue.Showtime?.Theater?.Name ?? "N/A"}";
            ViewData["ShowtimeId"] = new SelectList(new[] { new { revenue.ShowtimeId, DisplayText = showtimeInfo } }, "ShowtimeId", "DisplayText", revenue.ShowtimeId);
            return View(revenue);
        }

        // POST: Revenues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RevenueId,ShowtimeId,TotalAmount,AgencyCommission")] Revenue revenue)
        {
            if (id != revenue.RevenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(revenue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RevenueExists(revenue.RevenueId))
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
            ViewData["ShowtimeId"] = new SelectList(_context.Showtimes, "ShowtimeId", "ShowtimeId", revenue.ShowtimeId);
            return View(revenue);
        }

        // GET: Revenues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var revenue = await _context.Revenues
                .Include(r => r.Showtime)
                .FirstOrDefaultAsync(m => m.RevenueId == id);
            if (revenue == null)
            {
                return NotFound();
            }

            return View(revenue);
        }

        // POST: Revenues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var revenue = await _context.Revenues.FindAsync(id);
            if (revenue != null)
            {
                _context.Revenues.Remove(revenue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RevenueExists(int id)
        {
            return _context.Revenues.Any(e => e.RevenueId == id);
        }
    }
}
