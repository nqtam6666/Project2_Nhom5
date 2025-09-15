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
    public class PaymentsController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public PaymentsController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index(string search, string paymentMethod, string status, string startDate, string endDate, string sortOrder, string currentSort, int page = 1, int pageSize = 10)
        {
            // Get all payments with includes
            var payments = _context.Payments
                .Include(p => p.Ticket)
                .ThenInclude(t => t.User)
                .Include(p => p.Ticket)
                .ThenInclude(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                payments = payments.Where(p => 
                    p.PaymentId.ToString().Contains(search) ||
                    p.Ticket.TicketId.ToString().Contains(search) ||
                    p.Ticket.User.Username.Contains(search) ||
                    p.Ticket.User.Email.Contains(search) ||
                    p.Ticket.Showtime.Movie.Title.Contains(search)
                );
            }

            // Apply payment method filter
            if (!string.IsNullOrEmpty(paymentMethod))
            {
                payments = payments.Where(p => p.PaymentMethod == paymentMethod);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                payments = payments.Where(p => p.Ticket.Status == status);
            }

            // Apply date range filter
            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out DateTime startDateFilter))
            {
                payments = payments.Where(p => p.PaymentDate >= startDateFilter);
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out DateTime endDateFilter))
            {
                endDateFilter = endDateFilter.AddDays(1).AddSeconds(-1); // End of day
                payments = payments.Where(p => p.PaymentDate <= endDateFilter);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(currentSort))
            {
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            }

            payments = currentSort switch
            {
                "amount" => sortOrder == "desc" ? payments.OrderByDescending(p => p.Amount) : payments.OrderBy(p => p.Amount),
                "paymentDate" => sortOrder == "desc" ? payments.OrderByDescending(p => p.PaymentDate) : payments.OrderBy(p => p.PaymentDate),
                "paymentMethod" => sortOrder == "desc" ? payments.OrderByDescending(p => p.PaymentMethod) : payments.OrderBy(p => p.PaymentMethod),
                "status" => sortOrder == "desc" ? payments.OrderByDescending(p => p.Ticket.Status) : payments.OrderBy(p => p.Ticket.Status),
                "user" => sortOrder == "desc" ? payments.OrderByDescending(p => p.Ticket.User.Username) : payments.OrderBy(p => p.Ticket.User.Username),
                _ => payments.OrderByDescending(p => p.PaymentDate)
            };

            // Get total count for pagination
            var totalCount = await payments.CountAsync();

            // Apply pagination
            var skip = (page - 1) * pageSize;
            var pagedPayments = await payments.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.Status = status;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentSort = currentSort;

            return View(pagedPayments);
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Ticket)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public async Task<IActionResult> Create()
        {
            // Chỉ hiển thị những vé chưa có thanh toán và đang chờ xử lý
            var availableTickets = await _context.Tickets
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .Where(t => t.Status == "ChoXuLy" && !_context.Payments.Any(p => p.TicketId == t.TicketId))
                .Select(t => new { 
                    t.TicketId, 
                    DisplayText = $"Vé #{t.TicketId} - {(t.Showtime != null && t.Showtime.Movie != null ? t.Showtime.Movie.Title : "N/A")} ({(t.Showtime != null ? $"{t.Showtime.ShowDate:dd/MM/yyyy} {t.Showtime.ShowTime:HH:mm}" : "N/A")})" 
                })
                .ToListAsync();

            ViewData["TicketId"] = new SelectList(availableTickets, "TicketId", "DisplayText");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,TicketId,Amount,PaymentMethod,PaymentDate")] Payment payment)
        {
            try
            {
                // Kiểm tra xem vé đã có thanh toán chưa
                var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.TicketId == payment.TicketId);
                if (existingPayment != null)
                {
                    ModelState.AddModelError("TicketId", "Vé này đã có thanh toán. Mỗi vé chỉ có thể có một thanh toán.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(payment);
                    // Sau khi thanh toán, cập nhật trạng thái vé thành 'DaThanhToan'
                    var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == payment.TicketId);
                    if (ticket != null)
                    {
                        ticket.Status = "DaThanhToan";
                        _context.Tickets.Update(ticket);
                    }
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo thanh toán thành công!" });
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
                    string userMessage = "Có lỗi xảy ra khi tạo thanh toán";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể tạo thanh toán vì vé không tồn tại hoặc đã bị xóa. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Thanh toán này đã tồn tại cho vé này. Vui lòng kiểm tra lại.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            ViewData["TicketId"] = new SelectList(_context.Tickets, "TicketId", "TicketId", payment.TicketId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Ticket)
                .ThenInclude(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
            
            if (payment == null)
            {
                return NotFound();
            }

            // Hiển thị thông tin vé hiện tại
            var ticketInfo = $"Vé #{payment.TicketId?.ToString() ?? "N/A"} - {payment.Ticket?.Showtime?.Movie?.Title ?? "N/A"} ({payment.Ticket?.Showtime?.ShowDate:dd/MM/yyyy} {payment.Ticket?.Showtime?.ShowTime:HH:mm})";
            ViewData["TicketId"] = new SelectList(new[] { new { payment.TicketId, DisplayText = ticketInfo } }, "TicketId", "DisplayText", payment.TicketId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,TicketId,Amount,PaymentMethod,PaymentDate")] Payment payment)
        {
            try
            {
                if (id != payment.PaymentId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(payment);
                        await _context.SaveChangesAsync();
                        
                        // Check if request wants JSON response
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = true, message = "Cập nhật thanh toán thành công!" });
                        }
                        
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PaymentExists(payment.PaymentId))
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
                    string userMessage = "Có lỗi xảy ra khi cập nhật thanh toán";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể cập nhật thanh toán vì vé không tồn tại hoặc đã bị xóa. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Thanh toán này đã tồn tại cho vé này. Vui lòng kiểm tra lại.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            ViewData["TicketId"] = new SelectList(_context.Tickets, "TicketId", "TicketId", payment.TicketId ?? 0);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Ticket)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Xóa thanh toán thành công!" });
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy thanh toán để xóa" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi xóa thanh toán";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa thanh toán vì có dữ liệu liên quan. Vui lòng kiểm tra lại.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }
    }
}
