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
    public class TicketsController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public TicketsController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index(string search, string status, string userId, string showtimeId, string sortOrder, string currentSort, int page = 1, int pageSize = 10)
        {
            // Get all tickets with includes
            var tickets = _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Theater)
                .Include(t => t.User)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                tickets = tickets.Where(t => 
                    t.User.Username.Contains(search) || 
                    t.User.Email.Contains(search) ||
                    t.Showtime.Movie.Title.Contains(search) ||
                    t.Showtime.Theater.Name.Contains(search) ||
                    t.Seat.SeatCode.Contains(search)
                );
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                tickets = tickets.Where(t => t.Status == status);
            }

            // Apply user filter
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                tickets = tickets.Where(t => t.UserId == userIdInt);
            }

            // Apply showtime filter
            if (!string.IsNullOrEmpty(showtimeId) && int.TryParse(showtimeId, out int showtimeIdInt))
            {
                tickets = tickets.Where(t => t.ShowtimeId == showtimeIdInt);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(currentSort))
            {
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            }

            tickets = currentSort switch
            {
                "user" => sortOrder == "desc" ? tickets.OrderByDescending(t => t.User.Username) : tickets.OrderBy(t => t.User.Username),
                "movie" => sortOrder == "desc" ? tickets.OrderByDescending(t => t.Showtime.Movie.Title) : tickets.OrderBy(t => t.Showtime.Movie.Title),
                "theater" => sortOrder == "desc" ? tickets.OrderByDescending(t => t.Showtime.Theater.Name) : tickets.OrderBy(t => t.Showtime.Theater.Name),
                "price" => sortOrder == "desc" ? tickets.OrderByDescending(t => t.Price) : tickets.OrderBy(t => t.Price),
                "status" => sortOrder == "desc" ? tickets.OrderByDescending(t => t.Status) : tickets.OrderBy(t => t.Status),
                _ => tickets.OrderByDescending(t => t.TicketId)
            };

            // Get total count for pagination
            var totalCount = await tickets.CountAsync();

            // Apply pagination
            var skip = (page - 1) * pageSize;
            var pagedTickets = await tickets.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.UserId = userId;
            ViewBag.ShowtimeId = showtimeId;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentSort = currentSort;

            // Get users and showtimes for filter dropdowns
            ViewBag.Users = await _context.Users.OrderBy(u => u.Username).ToListAsync();
            ViewBag.Showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .OrderByDescending(s => s.ShowDate)
                .ThenBy(s => s.ShowTime)
                .ToListAsync();

            return View(pagedTickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["SeatId"] = new SelectList(_context.Seats, "SeatId", "SeatId");
            ViewData["ShowtimeId"] = new SelectList(_context.Showtimes, "ShowtimeId", "ShowtimeId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,UserId,ShowtimeId,SeatId,Price,Status")] Ticket ticket)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo vé thành công!" });
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
                    string userMessage = "Có lỗi xảy ra khi tạo vé";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể tạo vé vì lịch chiếu hoặc ghế không tồn tại. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Vé này đã được đặt cho lịch chiếu và ghế này. Vui lòng chọn ghế khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            ViewData["SeatId"] = new SelectList(_context.Seats, "SeatId", "SeatId", ticket.SeatId);
            ViewData["ShowtimeId"] = new SelectList(_context.Showtimes, "ShowtimeId", "ShowtimeId", ticket.ShowtimeId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", ticket.UserId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["SeatId"] = new SelectList(_context.Seats, "SeatId", "SeatId", ticket.SeatId);
            ViewData["ShowtimeId"] = new SelectList(_context.Showtimes, "ShowtimeId", "ShowtimeId", ticket.ShowtimeId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", ticket.UserId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,UserId,ShowtimeId,SeatId,Price,Status")] Ticket ticket)
        {
            try
            {
                if (id != ticket.TicketId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(ticket);
                        await _context.SaveChangesAsync();
                        
                        // Check if request wants JSON response
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = true, message = "Cập nhật vé thành công!" });
                        }
                        
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TicketExists(ticket.TicketId))
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
                    string userMessage = "Có lỗi xảy ra khi cập nhật vé";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể cập nhật vé vì lịch chiếu hoặc ghế không tồn tại. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Vé này đã được đặt cho lịch chiếu và ghế này. Vui lòng chọn ghế khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            ViewData["SeatId"] = new SelectList(_context.Seats, "SeatId", "SeatId", ticket.SeatId);
            ViewData["ShowtimeId"] = new SelectList(_context.Showtimes, "ShowtimeId", "ShowtimeId", ticket.ShowtimeId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", ticket.UserId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket != null)
                {
                    _context.Tickets.Remove(ticket);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Xóa vé thành công!" });
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy vé để xóa" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi xóa vé";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa vé vì đã có thanh toán. Vui lòng xóa thanh toán trước.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}
