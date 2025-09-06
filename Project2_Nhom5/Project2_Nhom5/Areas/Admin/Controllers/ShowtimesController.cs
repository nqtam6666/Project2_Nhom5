using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;

namespace Project2_Nhom5.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShowtimesController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public ShowtimesController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Admin/Showtimes
        public async Task<IActionResult> Index(string search, string movieId, string theaterId, string showDate, string sortOrder, string currentSort, int page = 1, int pageSize = 10)
        {
            // Get all showtimes with includes
            var showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                showtimes = showtimes.Where(s => 
                    s.Movie.Title.Contains(search) || 
                    s.Theater.Name.Contains(search) ||
                    s.Theater.Location.Contains(search)
                );
            }

            // Apply movie filter
            if (!string.IsNullOrEmpty(movieId) && int.TryParse(movieId, out int movieIdInt))
            {
                showtimes = showtimes.Where(s => s.MovieId == movieIdInt);
            }

            // Apply theater filter
            if (!string.IsNullOrEmpty(theaterId) && int.TryParse(theaterId, out int theaterIdInt))
            {
                showtimes = showtimes.Where(s => s.TheaterId == theaterIdInt);
            }

            // Apply date filter
            if (!string.IsNullOrEmpty(showDate) && DateTime.TryParse(showDate, out DateTime dateFilter))
            {
                var dateOnlyFilter = DateOnly.FromDateTime(dateFilter);
                showtimes = showtimes.Where(s => s.ShowDate == dateOnlyFilter);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(currentSort))
            {
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            }

            showtimes = currentSort switch
            {
                "movie" => sortOrder == "desc" ? showtimes.OrderByDescending(s => s.Movie.Title) : showtimes.OrderBy(s => s.Movie.Title),
                "theater" => sortOrder == "desc" ? showtimes.OrderByDescending(s => s.Theater.Name) : showtimes.OrderBy(s => s.Theater.Name),
                "date" => sortOrder == "desc" ? showtimes.OrderByDescending(s => s.ShowDate) : showtimes.OrderBy(s => s.ShowDate),
                "time" => sortOrder == "desc" ? showtimes.OrderByDescending(s => s.ShowTime) : showtimes.OrderBy(s => s.ShowTime),
                _ => showtimes.OrderByDescending(s => s.ShowDate).ThenBy(s => s.ShowTime)
            };

            // Get total count for pagination
            var totalCount = await showtimes.CountAsync();

            // Apply pagination
            var skip = (page - 1) * pageSize;
            var pagedShowtimes = await showtimes.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;
            ViewBag.MovieId = movieId;
            ViewBag.TheaterId = theaterId;
            ViewBag.ShowDate = showDate;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentSort = currentSort;

            // Get movies and theaters for filter dropdowns
            ViewBag.Movies = await _context.Movies.OrderBy(m => m.Title).ToListAsync();
            ViewBag.Theaters = await _context.Theaters.OrderBy(t => t.Name).ToListAsync();

            return View(pagedShowtimes);
        }

        // GET: Admin/Showtimes/Create
        public async Task<IActionResult> Create()
        {
            ViewData["MovieId"] = new SelectList(await _context.Movies.ToListAsync(), "MovieId", "Title");
            ViewData["TheaterId"] = new SelectList(await _context.Theaters.ToListAsync(), "TheaterId", "Name");
            return View();
        }

        // POST: Admin/Showtimes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateShowtimeRequest request)
        {
            try
            {
                // Log the received data
                Console.WriteLine($"Received data: MovieId={request.MovieId}, TheaterId={request.TheaterId}, ShowDate={request.ShowDate}, ShowTime={request.ShowTime}");
                
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                    // Kiểm tra xem có lịch chiếu trùng không
                    var existingShowtime = await _context.Showtimes
                    .FirstOrDefaultAsync(s => s.MovieId == request.MovieId 
                                          && s.TheaterId == request.TheaterId 
                                          && s.ShowDate == request.ShowDate 
                                          && s.ShowTime == request.ShowTime);
                    
                    if (existingShowtime != null)
                    {
                    return Json(new { success = false, message = "Đã tồn tại lịch chiếu này trong hệ thống!" });
                }

                var showtime = new Showtime
                {
                    MovieId = request.MovieId,
                    TheaterId = request.TheaterId,
                    ShowDate = request.ShowDate,
                    ShowTime = request.ShowTime
                };

                    _context.Add(showtime);
                    await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Tạo lịch chiếu thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return Json(new { success = false, message = $"Lỗi khi tạo lịch chiếu: {ex.Message}" });
            }
        }

        // GET: Admin/Showtimes/BulkCreate
        public async Task<IActionResult> BulkCreate()
        {
            ViewData["MovieId"] = new SelectList(await _context.Movies.ToListAsync(), "MovieId", "Title");
            ViewData["TheaterId"] = new SelectList(await _context.Theaters.ToListAsync(), "TheaterId", "Name");
            return View();
        }

        // POST: Admin/Showtimes/BulkCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkCreate([FromBody] BulkShowtimeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var showtimes = new List<Showtime>();
                var startDate = request.StartDate;
                var endDate = request.EndDate;

                // Tạo lịch chiếu cho từng ngày trong khoảng thời gian
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    // Chỉ tạo lịch chiếu cho các ngày được chọn
                    if (request.SelectedDays.Contains((int)date.DayOfWeek))
                    {
                        foreach (var time in request.ShowTimes)
                        {
                            var showtime = new Showtime
                            {
                                MovieId = request.MovieId,
                                TheaterId = request.TheaterId,
                                ShowDate = DateOnly.FromDateTime(date),
                                ShowTime = TimeOnly.Parse(time)
                            };
                            showtimes.Add(showtime);
                        }
                    }
                }

                _context.Showtimes.AddRange(showtimes);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"Đã tạo thành công {showtimes.Count} lịch chiếu",
                    count = showtimes.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: Admin/Showtimes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(await _context.Movies.ToListAsync(), "MovieId", "Title", showtime.MovieId);
            ViewData["TheaterId"] = new SelectList(await _context.Theaters.ToListAsync(), "TheaterId", "Name", showtime.TheaterId);
            return View(showtime);
        }

        // POST: Admin/Showtimes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] EditShowtimeRequest request)
        {
            try
            {
                // Log the received data
                Console.WriteLine($"Edit - Received data: ShowtimeId={request.ShowtimeId}, MovieId={request.MovieId}, TheaterId={request.TheaterId}, ShowDate={request.ShowDate}, ShowTime={request.ShowTime}");
                
                if (id != request.ShowtimeId)
                {
                    return Json(new { success = false, message = "ID không khớp" });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                    // Kiểm tra xem có lịch chiếu trùng không (trừ chính nó)
                    var existingShowtime = await _context.Showtimes
                    .FirstOrDefaultAsync(s => s.ShowtimeId != request.ShowtimeId
                                          && s.MovieId == request.MovieId 
                                          && s.TheaterId == request.TheaterId 
                                          && s.ShowDate == request.ShowDate 
                                          && s.ShowTime == request.ShowTime);
                    
                    if (existingShowtime != null)
                    {
                    return Json(new { success = false, message = "Đã tồn tại lịch chiếu này trong hệ thống!" });
                }

                var showtime = await _context.Showtimes.FindAsync(request.ShowtimeId);
                if (showtime == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lịch chiếu" });
                }

                showtime.MovieId = request.MovieId;
                showtime.TheaterId = request.TheaterId;
                showtime.ShowDate = request.ShowDate;
                showtime.ShowTime = request.ShowTime;

                    _context.Update(showtime);
                    await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Cập nhật lịch chiếu thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit - Exception: {ex.Message}");
                Console.WriteLine($"Edit - Inner exception: {ex.InnerException?.Message}");
                return Json(new { success = false, message = $"Lỗi khi cập nhật lịch chiếu: {ex.Message}" });
            }
        }

        // GET: Admin/Showtimes/Delete/5
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

        // POST: Admin/Showtimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime != null)
            {
                _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Xóa lịch chiếu thành công!" });
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy lịch chiếu để xóa" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi xóa lịch chiếu";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa lịch chiếu vì đã có vé được đặt. Vui lòng hủy vé trước.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool ShowtimeExists(int id)
        {
            return _context.Showtimes.Any(e => e.ShowtimeId == id);
        }
    }

    public class CreateShowtimeRequest
    {
        [Required]
        public int MovieId { get; set; }
        
        [Required]
        public int TheaterId { get; set; }
        
        [Required]
        public DateOnly ShowDate { get; set; }
        
        [Required]
        public TimeOnly ShowTime { get; set; }
    }

    public class EditShowtimeRequest
    {
        [Required]
        public int ShowtimeId { get; set; }
        
        [Required]
        public int MovieId { get; set; }
        
        [Required]
        public int TheaterId { get; set; }
        
        [Required]
        public DateOnly ShowDate { get; set; }
        
        [Required]
        public TimeOnly ShowTime { get; set; }
    }

    public class BulkShowtimeRequest
    {
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> SelectedDays { get; set; } = new List<int>();
        public List<string> ShowTimes { get; set; } = new List<string>();
    }
}
