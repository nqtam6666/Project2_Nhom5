using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;
using Project2_Nhom5.Areas.Guest.Models;
using System.Text.RegularExpressions;

namespace Project2_Nhom5.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class BookingController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public BookingController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // Debug action để xem tất cả suất chiếu có sẵn
        [HttpGet]
        public async Task<IActionResult> DebugShowtimes()
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .OrderBy(s => s.ShowtimeId)
                .ToListAsync();

            var result = showtimes.Select(s => new
            {
                ShowtimeId = s.ShowtimeId,
                MovieTitle = s.Movie?.Title,
                TheaterName = s.Theater?.Name,
                ShowDate = s.ShowDate.ToString("yyyy-MM-dd"),
                ShowTime = s.ShowTime.ToString(@"hh\:mm"),
                IsToday = s.ShowDate == DateOnly.FromDateTime(DateTime.Today),
                IsFuture = s.ShowDate >= DateOnly.FromDateTime(DateTime.Today)
            }).ToList();

            return Json(new { 
                total = result.Count,
                showtimes = result,
                today = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd")
            });
        }

        // Hiển thị giao diện chọn suất chiếu
        [HttpGet]
        public async Task<IActionResult> SelectShowtime(int movieId)
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null || movie.Status != "dangchieu")
            {
                return NotFound();
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);

            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .Where(s => s.MovieId == movieId && 
                           (s.ShowDate > today || 
                            (s.ShowDate == today && s.ShowTime > now)))
                .OrderBy(s => s.ShowDate)
                .ThenBy(s => s.ShowTime)
                .ToListAsync();

            ViewData["Movie"] = movie;
            ViewData["IsLoggedIn"] = true;
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(showtimes);
        }

        // Hiển thị giao diện chọn ghế
        [HttpGet]
        public async Task<IActionResult> SelectSeats(int showtimeId)
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                return NotFound();
            }

            // Kiểm tra ngày chiếu
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);
            
            // Chỉ cho phép đặt vé cho suất chiếu từ hôm nay trở đi
            // Nếu là hôm nay thì phải chưa đến giờ chiếu
            if (showtime.ShowDate < today || 
                (showtime.ShowDate == today && showtime.ShowTime <= now))
            {
                return NotFound();
            }

            // Lấy tất cả ghế của rạp
            var allSeats = await _context.Seats
                .Where(s => s.TheaterId == showtime.TheaterId)
                .OrderBy(s => s.SeatCode)
                .ToListAsync();

            // Lấy ghế đã được đặt
            var bookedSeatIds = await _context.Tickets
                .Where(t => t.ShowtimeId == showtimeId && t.Status != "dahuy")
                .Select(t => t.SeatId)
                .ToListAsync();

            var seatViewModels = allSeats.Select(seat =>
            {
                var match = Regex.Match(seat.SeatCode, @"^([A-Z])(\d+)$");
                var row = match.Success ? match.Groups[1].Value : "";
                var column = match.Success ? int.Parse(match.Groups[2].Value) : 0;

                return new SeatViewModel
                {
                    SeatId = seat.SeatId,
                    SeatCode = seat.SeatCode,
                    SeatType = seat.SeatType ?? "thuong",
                    IsBooked = bookedSeatIds.Contains(seat.SeatId),
                    IsSelected = false,
                    Price = seat.SeatType == "VIP" ? 95000 : 75000,
                    Row = row,
                    Column = column
                };
            }).ToList();

            var viewModel = new SeatSelectionViewModel
            {
                ShowtimeId = showtime.ShowtimeId,
                MovieId = showtime.MovieId,
                MovieTitle = showtime.Movie?.Title ?? "",
                TheaterName = showtime.Theater?.Name ?? "",
                ShowDate = showtime.ShowDate.ToDateTime(TimeOnly.MinValue),
                ShowTime = TimeSpan.FromTicks(showtime.ShowTime.Ticks),
                MoviePoster = showtime.Movie?.PosterUrl ?? "",
                AvailableSeats = seatViewModels.Where(s => !s.IsBooked).ToList(),
                BookedSeats = seatViewModels.Where(s => s.IsBooked).ToList(),
                BasePrice = 75000
            };

            ViewData["IsLoggedIn"] = true;
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(viewModel);
        }

        // API để lấy thông tin ghế đã chọn
        [HttpPost]
        public async Task<IActionResult> GetSelectedSeats([FromBody] List<int> seatIds)
        {
            if (seatIds == null || !seatIds.Any())
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một ghế" });
            }

            var seats = await _context.Seats
                .Where(s => seatIds.Contains(s.SeatId))
                .Select(s => new
                {
                    s.SeatId,
                    s.SeatCode,
                    s.SeatType,
                    Price = s.SeatType == "VIP" ? 95000 : 75000
                })
                .ToListAsync();

            var totalPrice = seats.Sum(s => s.Price);

            return Json(new
            {
                success = true,
                seats = seats,
                totalPrice = totalPrice,
                seatCount = seats.Count
            });
        }

        // Hiển thị giao diện thanh toán
        [HttpGet]
        public async Task<IActionResult> Payment(int showtimeId, [FromQuery] string seatIds)
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(seatIds))
            {
                return RedirectToAction("SelectSeats", new { showtimeId });
            }

            var selectedSeatIds = seatIds.Split(',').Select(int.Parse).ToList();

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                return NotFound();
            }

            var selectedSeats = await _context.Seats
                .Where(s => selectedSeatIds.Contains(s.SeatId))
                .Select(s => new SeatViewModel
                {
                    SeatId = s.SeatId,
                    SeatCode = s.SeatCode,
                    SeatType = s.SeatType ?? "thuong",
                    Price = s.SeatType == "VIP" ? 95000 : 75000
                })
                .ToListAsync();

            var subtotal = selectedSeats.Sum(s => s.Price);

            var viewModel = new PaymentViewModel
            {
                ShowtimeId = showtime.ShowtimeId,
                MovieTitle = showtime.Movie?.Title ?? "",
                TheaterName = showtime.Theater?.Name ?? "",
                ShowDate = showtime.ShowDate.ToDateTime(TimeOnly.MinValue),
                ShowTime = TimeSpan.FromTicks(showtime.ShowTime.Ticks),
                MoviePoster = showtime.Movie?.PosterUrl ?? "",
                SelectedSeats = selectedSeats,
                SubTotal = subtotal,
                DiscountAmount = 0,
                TotalAmount = subtotal
            };

            ViewData["IsLoggedIn"] = true;
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(viewModel);
        }

        // API để áp dụng mã giảm giá
        [HttpPost]
        public async Task<IActionResult> ApplyDiscount([FromBody] DiscountRequest request)
        {
            if (string.IsNullOrEmpty(request.DiscountCode))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá" });
            }

            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.Code == request.DiscountCode && d.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today));

            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn" });
            }

            decimal discountAmount = 0;
            if (discount.DiscountType == "phantram")
            {
                discountAmount = request.SubTotal * (discount.Value / 100);
            }
            else
            {
                discountAmount = discount.Value;
            }

            var totalAmount = request.SubTotal - discountAmount;

            return Json(new
            {
                success = true,
                discountAmount = discountAmount,
                totalAmount = totalAmount,
                discountType = discount.DiscountType,
                discountValue = discount.Value
            });
        }

        // Xử lý thanh toán và tạo vé
        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để đặt vé" });
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Kiểm tra ghế có còn trống không
                var bookedSeats = await _context.Tickets
                    .Where(t => t.ShowtimeId == request.ShowtimeId && 
                               request.SeatIds.Contains(t.SeatId) && 
                               t.Status != "dahuy")
                    .ToListAsync();

                if (bookedSeats.Any())
                {
                    return Json(new { success = false, message = "Một số ghế đã được đặt. Vui lòng chọn ghế khác." });
                }

                var userIdInt = int.Parse(userId);
                var tickets = new List<Ticket>();

                // Tạo vé cho từng ghế
                foreach (var seatId in request.SeatIds)
                {
                    var seat = await _context.Seats.FindAsync(seatId);
                    var price = seat?.SeatType == "VIP" ? 95000 : 75000;

                    var ticket = new Ticket
                    {
                        UserId = userIdInt,
                        ShowtimeId = request.ShowtimeId,
                        SeatId = seatId,
                        Price = price,
                        Status = "choxuly"
                    };

                    tickets.Add(ticket);
                }

                _context.Tickets.AddRange(tickets);
                await _context.SaveChangesAsync();

                // Tạo thanh toán
                var payment = new Payment
                {
                    TicketId = tickets.First().TicketId, // Thanh toán cho vé đầu tiên
                    Amount = request.TotalAmount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentDate = DateTime.Now
                };

                _context.Payments.Add(payment);

                // Cập nhật trạng thái vé
                foreach (var ticket in tickets)
                {
                    ticket.Status = "dathanhtoan";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Tạo mã đặt vé
                var bookingCode = $"BK{DateTime.Now:yyyyMMdd}{tickets.First().TicketId:D6}";

                return Json(new
                {
                    success = true,
                    message = "Đặt vé thành công!",
                    bookingCode = bookingCode,
                    ticketIds = tickets.Select(t => t.TicketId).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // Hiển thị xác nhận đặt vé
        [HttpGet]
        public async Task<IActionResult> Confirmation(int ticketId)
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var ticket = await _context.Tickets
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Theater)
                .Include(t => t.Seat)
                .Include(t => t.Payment)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId && t.UserId.ToString() == userId);

            if (ticket == null)
            {
                return NotFound();
            }

            var bookingCode = $"BK{DateTime.Now:yyyyMMdd}{ticket.TicketId:D6}";

            var viewModel = new BookingConfirmationViewModel
            {
                TicketId = ticket.TicketId,
                MovieTitle = ticket.Showtime?.Movie?.Title ?? "",
                TheaterName = ticket.Showtime?.Theater?.Name ?? "",
                ShowDate = ticket.Showtime?.ShowDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                ShowTime = ticket.Showtime != null ? TimeSpan.FromTicks(ticket.Showtime.ShowTime.Ticks) : TimeSpan.Zero,
                SeatCodes = new List<string> { ticket.Seat?.SeatCode ?? "" },
                TotalAmount = ticket.Payment?.Amount ?? 0,
                PaymentMethod = ticket.Payment?.PaymentMethod ?? "",
                BookingDate = ticket.Payment?.PaymentDate ?? DateTime.Now,
                BookingCode = bookingCode
            };

            ViewData["IsLoggedIn"] = true;
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(viewModel);
        }

        // Hiển thị lịch sử đặt vé
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var tickets = await _context.Tickets
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Theater)
                .Include(t => t.Seat)
                .Include(t => t.Payment)
                .Where(t => t.UserId.ToString() == userId)
                .OrderByDescending(t => t.Payment.PaymentDate)
                .ToListAsync();

            var viewModels = tickets.Select(ticket => new BookingHistoryViewModel
            {
                TicketId = ticket.TicketId,
                MovieTitle = ticket.Showtime?.Movie?.Title ?? "",
                TheaterName = ticket.Showtime?.Theater?.Name ?? "",
                ShowDate = ticket.Showtime?.ShowDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                ShowTime = ticket.Showtime != null ? TimeSpan.FromTicks(ticket.Showtime.ShowTime.Ticks) : TimeSpan.Zero,
                SeatCodes = new List<string> { ticket.Seat?.SeatCode ?? "" },
                TotalAmount = ticket.Payment?.Amount ?? 0,
                Status = ticket.Status ?? "",
                BookingDate = ticket.Payment?.PaymentDate ?? DateTime.Now,
                PaymentMethod = ticket.Payment?.PaymentMethod
            }).ToList();

            ViewData["IsLoggedIn"] = true;
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(viewModels);
        }
    }

    // Request models
    public class DiscountRequest
    {
        public string DiscountCode { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
    }

    public class PaymentRequest
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? DiscountCode { get; set; }
    }
} 