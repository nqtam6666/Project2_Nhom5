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
        public async Task<IActionResult> Index(string search, string theaterId, string movieId, string startDate, string endDate, string sortOrder, string currentSort, int page = 1, int pageSize = 10)
        {
            // Get all revenues with includes
            var revenues = _context.Revenues
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Theater)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                revenues = revenues.Where(r => 
                    r.RevenueId.ToString().Contains(search) ||
                    r.Showtime.ShowtimeId.ToString().Contains(search) ||
                    r.Showtime.Movie.Title.Contains(search) ||
                    r.Showtime.Theater.Name.Contains(search) ||
                    r.Showtime.Theater.Location.Contains(search)
                );
            }

            // Apply theater filter
            if (!string.IsNullOrEmpty(theaterId) && int.TryParse(theaterId, out int theaterIdInt))
            {
                revenues = revenues.Where(r => r.Showtime.TheaterId == theaterIdInt);
            }

            // Apply movie filter
            if (!string.IsNullOrEmpty(movieId) && int.TryParse(movieId, out int movieIdInt))
            {
                revenues = revenues.Where(r => r.Showtime.MovieId == movieIdInt);
            }

            // Apply date range filter
            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out DateTime startDateFilter))
            {
                revenues = revenues.Where(r => r.Showtime.ShowDate >= DateOnly.FromDateTime(startDateFilter));
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out DateTime endDateFilter))
            {
                revenues = revenues.Where(r => r.Showtime.ShowDate <= DateOnly.FromDateTime(endDateFilter));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(currentSort))
            {
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            }

            revenues = currentSort switch
            {
                "totalAmount" => sortOrder == "desc" ? revenues.OrderByDescending(r => r.TotalAmount) : revenues.OrderBy(r => r.TotalAmount),
                "agencyCommission" => sortOrder == "desc" ? revenues.OrderByDescending(r => r.AgencyCommission) : revenues.OrderBy(r => r.AgencyCommission),
                "showDate" => sortOrder == "desc" ? revenues.OrderByDescending(r => r.Showtime.ShowDate) : revenues.OrderBy(r => r.Showtime.ShowDate),
                "movie" => sortOrder == "desc" ? revenues.OrderByDescending(r => r.Showtime.Movie.Title) : revenues.OrderBy(r => r.Showtime.Movie.Title),
                "theater" => sortOrder == "desc" ? revenues.OrderByDescending(r => r.Showtime.Theater.Name) : revenues.OrderBy(r => r.Showtime.Theater.Name),
                _ => revenues.OrderByDescending(r => r.RevenueId)
            };

            // Get total count for pagination
            var totalCount = await revenues.CountAsync();

            // Apply pagination
            var skip = (page - 1) * pageSize;
            var pagedRevenues = await revenues.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;
            ViewBag.TheaterId = theaterId;
            ViewBag.MovieId = movieId;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentSort = currentSort;

            // Get theaters and movies for filter dropdowns
            ViewBag.Theaters = await _context.Theaters.OrderBy(t => t.Name).ToListAsync();
            ViewBag.Movies = await _context.Movies.OrderBy(m => m.Title).ToListAsync();

            return View(pagedRevenues);
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
                .Select(s => new { 
                    s.ShowtimeId, 
                    DisplayText = $"#{s.ShowtimeId} - {(s.Movie != null ? s.Movie.Title : "N/A")} ({s.ShowDate:dd/MM/yyyy} {s.ShowTime:HH:mm}) - {(s.Theater != null ? s.Theater.Name : "N/A")}" 
                })
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
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(revenue);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo doanh thu thành công!" });
                    }
                    
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        return Json(new { success = false, message = string.Join(", ", errors) });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi tạo doanh thu";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể tạo doanh thu vì lịch chiếu không tồn tại hoặc đã bị xóa. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Doanh thu này đã tồn tại cho lịch chiếu này. Vui lòng kiểm tra lại.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }

            // Reload dropdown data
            var availableShowtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .AsEnumerable()
                .Select(s => new { 
                    s.ShowtimeId, 
                    DisplayText = $"#{s.ShowtimeId} - {(s.Movie != null ? s.Movie.Title : "N/A")} ({s.ShowDate:dd/MM/yyyy} {s.ShowTime:HH:mm}) - {(s.Theater != null ? s.Theater.Name : "N/A")}" 
                })
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
            try
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
                        
                        // Check if request wants JSON response
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = true, message = "Cập nhật doanh thu thành công!" });
                        }
                        
                        return RedirectToAction(nameof(Index));
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
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        return Json(new { success = false, message = string.Join(", ", errors) });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi cập nhật doanh thu";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể cập nhật doanh thu vì lịch chiếu không tồn tại hoặc đã bị xóa. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Doanh thu này đã tồn tại cho lịch chiếu này. Vui lòng kiểm tra lại.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
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
            try
            {
                var revenue = await _context.Revenues.FindAsync(id);
                if (revenue != null)
                {
                    _context.Revenues.Remove(revenue);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Xóa doanh thu thành công!" });
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy doanh thu để xóa" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi xóa doanh thu";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa doanh thu vì có dữ liệu liên quan. Vui lòng kiểm tra lại.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool RevenueExists(int id)
        {
            return _context.Revenues.Any(e => e.RevenueId == id);
        }
    }
}
