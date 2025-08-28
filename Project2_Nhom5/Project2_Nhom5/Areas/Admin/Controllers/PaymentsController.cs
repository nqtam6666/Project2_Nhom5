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
        public async Task<IActionResult> Index()
        {
            var project2_Nhom5Context = _context.Payments.Include(p => p.Ticket);
            return View(await project2_Nhom5Context.ToListAsync());
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
                .Where(t => t.Status == "choxuly" && !_context.Payments.Any(p => p.TicketId == t.TicketId))
                .Select(t => new { t.TicketId, DisplayText = $"Vé #{t.TicketId} - {t.Showtime.Movie.Title} ({t.Showtime.ShowDate:dd/MM/yyyy} {t.Showtime.ShowTime:HH:mm})" })
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
            // Kiểm tra xem vé đã có thanh toán chưa
            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.TicketId == payment.TicketId);
            if (existingPayment != null)
            {
                ModelState.AddModelError("TicketId", "Vé này đã có thanh toán. Mỗi vé chỉ có thể có một thanh toán.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(payment);
                // Sau khi thanh toán, cập nhật trạng thái vé thành 'dathanhtoan'
                var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == payment.TicketId);
                if (ticket != null)
                {
                    ticket.Status = "dathanhtoan";
                    _context.Tickets.Update(ticket);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
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
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }
    }
}
