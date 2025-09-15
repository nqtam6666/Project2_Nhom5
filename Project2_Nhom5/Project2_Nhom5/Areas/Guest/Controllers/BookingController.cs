using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;
using Project2_Nhom5.Areas.Guest.Models;
using Project2_Nhom5.Services;
using System.Text.RegularExpressions;

namespace Project2_Nhom5.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class BookingController : Controller
    {
        private readonly Project2_Nhom5Context _context;
        private readonly RevenueService _revenueService;

        public BookingController(Project2_Nhom5Context context, RevenueService revenueService)
        {
            _context = context;
            _revenueService = revenueService;
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
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem suất chiếu.";
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
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để chọn ghế.";
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
                .Where(t => t.ShowtimeId == showtimeId && t.Status != "DaHuy")
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
                    Price = (seat.SeatType ?? "thuong") == "VIP" ? 95000 : 75000,
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
                    Price = (s.SeatType ?? "thuong") == "VIP" ? 95000 : 75000
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
            try
            {
                var userId = Request.Cookies["userId"];
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Bạn cần đăng nhập để thực hiện thanh toán.";
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
                    Price = (s.SeatType ?? "thuong") == "VIP" ? 95000 : 75000
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
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in Payment action: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
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

        // Xử lý thanh toán và tạo vé (giữ nguyên tên ProcessPayment để tương thích)
        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            try
            {
                Console.WriteLine("=== ProcessPayment Debug ===");
                Console.WriteLine($"Request: {System.Text.Json.JsonSerializer.Serialize(request)}");
                Console.WriteLine($"UserId from cookie: {Request.Cookies["userId"]}");
                Console.WriteLine($"Username from cookie: {Request.Cookies["username"]}");
                Console.WriteLine($"Role from cookie: {Request.Cookies["role"]}");
                
                var result = await CreatePendingTickets(request);
                
                Console.WriteLine($"Result type: {result.GetType().Name}");
                if (result is JsonResult jsonResult)
                {
                    Console.WriteLine($"Json result: {System.Text.Json.JsonSerializer.Serialize(jsonResult.Value)}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProcessPayment Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return Json(new { 
                    success = false, 
                    message = "Có lỗi xảy ra trong ProcessPayment", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // Tạo vé chưa thanh toán (chỉ tạo vé với trạng thái ChoXuLy)
        [HttpPost]
        public async Task<IActionResult> CreatePendingTickets([FromBody] PaymentRequest request)
        {
            Console.WriteLine("=== CreatePendingTickets Debug ===");
            Console.WriteLine($"Request: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            var userId = Request.Cookies["userId"];
            Console.WriteLine($"UserId from cookie: {userId}");
            
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("No userId found in cookies");
                return Json(new { success = false, message = "Vui lòng đăng nhập để đặt vé" });
            }

            try
            {
                Console.WriteLine("Starting database transaction...");
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Kiểm tra ghế có còn trống không
                Console.WriteLine($"Checking seats for showtime {request.ShowtimeId}, seats: {string.Join(",", request.SeatIds)}");
                var bookedSeats = await _context.Tickets
                    .Where(t => t.ShowtimeId == request.ShowtimeId && 
                               request.SeatIds.Contains(t.SeatId) && 
                               t.Status != "DaHuy")
                    .ToListAsync();

                Console.WriteLine($"Found {bookedSeats.Count} booked seats");

                if (bookedSeats.Any())
                {
                    return Json(new { success = false, message = "Một số ghế đã được đặt. Vui lòng chọn ghế khác." });
                }

                var userIdInt = int.Parse(userId);
                var tickets = new List<Ticket>();

                Console.WriteLine("Creating tickets...");
                // Tạo vé cho từng ghế với trạng thái ChoXuLy
                foreach (var seatId in request.SeatIds)
                {
                    Console.WriteLine($"Processing seat {seatId}");
                    var seat = await _context.Seats.FindAsync(seatId);
                    var price = (seat?.SeatType ?? "thuong") == "VIP" ? 95000 : 75000;

                    var ticket = new Ticket
                    {
                        UserId = userIdInt,
                        ShowtimeId = request.ShowtimeId,
                        SeatId = seatId,
                        Price = price,
                        Status = "ChoXuLy"
                    };

                    tickets.Add(ticket);
                    Console.WriteLine($"Created ticket for seat {seatId}, price: {price}");
                }

                Console.WriteLine($"Adding {tickets.Count} tickets to context");
                _context.Tickets.AddRange(tickets);
                
                Console.WriteLine("Saving changes...");
                await _context.SaveChangesAsync();
                
                Console.WriteLine("Committing transaction...");
                await transaction.CommitAsync();

                Console.WriteLine("Transaction committed successfully");

                // Tạo mã đặt vé
                var bookingCode = $"BK{DateTime.Now:yyyyMMdd}{tickets.First().TicketId:D6}";

                var result = new
                {
                    success = true,
                    message = "Đặt vé thành công! Vui lòng thanh toán để hoàn tất.",
                    ticketIds = tickets.Select(t => t.TicketId).ToList(),
                    redirectUrl = Url.Action("PendingPayments", "Booking"),
                    bookingCode = bookingCode
                };

                Console.WriteLine($"Returning result: {System.Text.Json.JsonSerializer.Serialize(result)}");
                return Json(result);
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi để debug
                Console.WriteLine($"Error in CreatePendingTickets: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                string userMessage = "Có lỗi xảy ra khi đặt vé";
                
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    {
                        userMessage = "Lỗi tham chiếu dữ liệu. Vui lòng kiểm tra lại thông tin.";
                    }
                    else if (ex.InnerException.Message.Contains("UNIQUE constraint"))
                    {
                        userMessage = "Dữ liệu đã tồn tại. Vui lòng thử lại.";
                    }
                    else if (ex.InnerException.Message.Contains("CHECK constraint"))
                    {
                        userMessage = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.InnerException.Message.Contains("Invalid column name"))
                    {
                        userMessage = "Lỗi cấu trúc database. Vui lòng liên hệ quản trị viên.";
                    }
                }
                
                var errorResult = new { 
                    success = false, 
                    message = userMessage,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                };
                
                Console.WriteLine($"Returning error: {System.Text.Json.JsonSerializer.Serialize(errorResult)}");
                return Json(errorResult);
            }
        }

        // Hiển thị xác nhận đặt vé
        [HttpGet]
        public async Task<IActionResult> Confirmation(int ticketId)
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem xác nhận đặt vé.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Tạm thời không include Payment để tránh lỗi DiscountId
                var ticket = await _context.Tickets
                    .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                    .Include(t => t.Showtime)
                    .ThenInclude(s => s.Theater)
                    .Include(t => t.Seat)
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId && t.UserId.ToString() == userId);

                if (ticket == null)
                {
                    return NotFound();
                }

                // Tạo mã đặt vé
                var bookingCode = $"BK{DateTime.Now:yyyyMMdd}{ticket.TicketId:D6}";

                // Lấy thông tin payment riêng biệt nếu vé đã thanh toán
                Payment? payment = null;
                if (ticket.Status == "DaThanhToan")
                {
                    try
                    {
                        payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.TicketId == ticketId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading payment: {ex.Message}");
                        // Tiếp tục với payment = null
                    }
                }

                var viewModel = new BookingConfirmationViewModel
                {
                    TicketId = ticket.TicketId,
                    MovieTitle = ticket.Showtime?.Movie?.Title ?? "",
                    TheaterName = ticket.Showtime?.Theater?.Name ?? "",
                    ShowDate = ticket.Showtime?.ShowDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                    ShowTime = ticket.Showtime != null ? TimeSpan.FromTicks(ticket.Showtime.ShowTime.Ticks) : TimeSpan.Zero,
                    SeatCodes = new List<string> { ticket.Seat?.SeatCode ?? "" },
                    TotalAmount = payment?.Amount ?? ticket.Price, // Sử dụng giá vé nếu chưa có payment
                    PaymentMethod = payment?.PaymentMethod ?? (ticket.Status == "DaThanhToan" ? "Đã thanh toán" : "Chưa thanh toán"),
                    BookingDate = payment?.PaymentDate ?? DateTime.Now,
                    BookingCode = bookingCode,
                    Status = ticket.Status ?? ""
                };

                ViewData["IsLoggedIn"] = true;
                ViewData["Username"] = Request.Cookies["username"];
                ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Confirmation: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin vé. Vui lòng thử lại.";
                return RedirectToAction("History", "Booking");
            }
        }

        // Hiển thị lịch sử đặt vé
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem lịch sử đặt vé.";
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
                .OrderByDescending(t => t.TicketId)
                .ToListAsync();

            var viewModels = tickets.Select(ticket => new BookingHistoryViewModel
            {
                TicketId = ticket.TicketId,
                MovieTitle = ticket.Showtime?.Movie?.Title ?? "",
                TheaterName = ticket.Showtime?.Theater?.Name ?? "",
                ShowDate = ticket.Showtime?.ShowDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                ShowTime = ticket.Showtime != null ? TimeSpan.FromTicks(ticket.Showtime.ShowTime.Ticks) : TimeSpan.Zero,
                SeatCodes = new List<string> { ticket.Seat?.SeatCode ?? "" },
                TotalAmount = ticket.Price, // Sử dụng giá vé thay vì Payment.Amount
                Status = ticket.Status ?? "",
                BookingDate = ticket.Payment?.PaymentDate ?? DateTime.Now, // Sử dụng ngày thanh toán hoặc ngày hiện tại
                PaymentMethod = ticket.Status == "DaThanhToan" ? 
                    (ticket.Payment?.PaymentMethod ?? "Đã thanh toán") : 
                    "Chưa thanh toán"
            }).ToList();

            ViewData["IsLoggedIn"] = true;
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(viewModels);
        }

        // Hiển thị danh sách vé chưa thanh toán
        [HttpGet]
        public async Task<IActionResult> PendingPayments()
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem vé chưa thanh toán.";
                return RedirectToAction("Index", "Home");
            }

            var pendingTickets = await _context.Tickets
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Theater)
                .Include(t => t.Seat)
                .Where(t => t.UserId.ToString() == userId && t.Status == "ChoXuLy")
                .OrderBy(t => t.Showtime.ShowDate)
                .ThenBy(t => t.Showtime.ShowTime)
                .ToListAsync();

            var viewModels = pendingTickets.Select(ticket => new PendingPaymentViewModel
            {
                TicketId = ticket.TicketId,
                MovieTitle = ticket.Showtime?.Movie?.Title ?? "",
                TheaterName = ticket.Showtime?.Theater?.Name ?? "",
                ShowDate = ticket.Showtime?.ShowDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                ShowTime = ticket.Showtime != null ? TimeSpan.FromTicks(ticket.Showtime.ShowTime.Ticks) : TimeSpan.Zero,
                SeatCode = ticket.Seat?.SeatCode ?? "",
                SeatType = ticket.Seat?.SeatType ?? "thuong",
                Price = ticket.Price,
                Status = ticket.Status ?? "",
                CreatedDate = DateTime.Now, // Có thể thêm trường CreatedDate vào Ticket model
                IsSelected = false
            }).ToList();

            var viewModel = new PendingPaymentsViewModel
            {
                PendingTickets = viewModels,
                TotalAmount = viewModels.Sum(t => t.Price),
                DiscountAmount = 0,
                FinalAmount = viewModels.Sum(t => t.Price)
            };

            ViewData["IsLoggedIn"] = true;
            ViewData["Username"] = Request.Cookies["username"];
            ViewData["IsAdmin"] = string.Equals(Request.Cookies["role"], "Admin", StringComparison.OrdinalIgnoreCase);

            return View(viewModel);
        }

        // API để lấy thông tin vé đã chọn
        [HttpPost]
        public async Task<IActionResult> GetSelectedTicketsInfo([FromBody] List<int> ticketIds)
        {
            if (ticketIds == null || !ticketIds.Any())
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một vé" });
            }

            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập" });
            }

            var tickets = await _context.Tickets
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Theater)
                .Include(t => t.Seat)
                .Where(t => ticketIds.Contains(t.TicketId) && 
                           t.UserId.ToString() == userId && 
                           t.Status == "ChoXuLy")
                .Select(t => new
                {
                    t.TicketId,
                    MovieTitle = t.Showtime.Movie.Title,
                    TheaterName = t.Showtime.Theater.Name,
                    ShowDate = t.Showtime.ShowDate.ToString("dd/MM/yyyy"),
                    ShowTime = t.Showtime.ShowTime.ToString(@"hh\:mm"),
                    SeatCode = t.Seat.SeatCode,
                    SeatType = t.Seat.SeatType,
                    t.Price
                })
                .ToListAsync();

            var totalPrice = tickets.Sum(t => t.Price);

            return Json(new
            {
                success = true,
                tickets = tickets,
                totalPrice = totalPrice,
                ticketCount = tickets.Count
            });
        }

        // API để áp dụng mã giảm giá cho vé đã chọn
        [HttpPost]
        public async Task<IActionResult> ApplyDiscountToSelected([FromBody] DiscountRequest request)
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
            if (discount.DiscountType == "percentage" || discount.DiscountType == "phantram")
            {
                discountAmount = request.SubTotal * (discount.Value / 100);
            }
            else if (discount.DiscountType == "fixed" || discount.DiscountType == "codinh")
            {
                discountAmount = discount.Value;
            }

            var totalAmount = Math.Max(0, request.SubTotal - discountAmount);

            return Json(new
            {
                success = true,
                discountAmount = discountAmount,
                totalAmount = totalAmount,
                discountType = discount.DiscountType,
                discountValue = discount.Value
            });
        }

        // Xử lý thanh toán cho vé đã chọn
        [HttpPost]
        public async Task<IActionResult> ProcessSelectedPayments([FromBody] PaymentSelectionRequest request)
        {
            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thanh toán" });
            }

            if (request.SelectedTicketIds == null || !request.SelectedTicketIds.Any())
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một vé để thanh toán" });
            }

            try
            {
                // Debug: Log thông tin request
                Console.WriteLine($"ProcessSelectedPayments - UserId: {userId}");
                Console.WriteLine($"ProcessSelectedPayments - SelectedTicketIds: {string.Join(",", request.SelectedTicketIds)}");
                Console.WriteLine($"ProcessSelectedPayments - PaymentMethod: {request.PaymentMethod}");
                Console.WriteLine($"ProcessSelectedPayments - DiscountCode: {request.DiscountCode}");

                using var transaction = await _context.Database.BeginTransactionAsync();

                // Lấy thông tin vé
                var tickets = await _context.Tickets
                    .Where(t => request.SelectedTicketIds.Contains(t.TicketId) && 
                               t.UserId.ToString() == userId && 
                               t.Status == "ChoXuLy")
                    .ToListAsync();

                Console.WriteLine($"Found {tickets.Count} tickets to process");

                if (!tickets.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy vé hợp lệ để thanh toán" });
                }

                // Tìm mã giảm giá nếu có
                int? discountId = null;
                Discount? discount = null;
                if (!string.IsNullOrEmpty(request.DiscountCode))
                {
                    discount = await _context.Discounts
                        .FirstOrDefaultAsync(d => d.Code == request.DiscountCode && d.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today));
                    if (discount != null)
                    {
                        discountId = discount.DiscountId;
                        Console.WriteLine($"Found discount: {discount.Code}, ID: {discountId}");
                    }
                    else
                    {
                        Console.WriteLine($"Discount not found: {request.DiscountCode}");
                    }
                }

                // Tính toán giá sau khi áp dụng mã giảm giá
                var totalOriginalPrice = tickets.Sum(t => t.Price);
                var finalAmount = totalOriginalPrice;
                var discountAmount = 0m;
                
                Console.WriteLine($"Total original price: {totalOriginalPrice}");
                
                if (discountId.HasValue && discount != null)
                {
                    Console.WriteLine($"Applying discount: {discount.Code}, Type: {discount.DiscountType}, Value: {discount.Value}");
                    
                    if (discount.DiscountType == "percentage" || discount.DiscountType == "phantram")
                    {
                        discountAmount = totalOriginalPrice * (discount.Value / 100);
                        finalAmount = totalOriginalPrice - discountAmount;
                    }
                    else if (discount.DiscountType == "fixed" || discount.DiscountType == "codinh")
                    {
                        discountAmount = discount.Value;
                        finalAmount = Math.Max(0, totalOriginalPrice - discountAmount);
                    }
                    
                    Console.WriteLine($"Discount amount: {discountAmount}, Final amount: {finalAmount}");
                }

                // Cập nhật vé với thông tin giảm giá
                foreach (var ticket in tickets)
                {
                    Console.WriteLine($"Updating status for ticket {ticket.TicketId} from {ticket.Status} to DaThanhToan");
                    
                    // Lưu giá gốc trước khi áp dụng giảm giá
                    ticket.OriginalPrice = ticket.Price;
                    
                    // Cập nhật giá vé sau khi áp dụng giảm giá
                    ticket.Price = finalAmount;
                    
                    // Lưu thông tin giảm giá
                    if (discount != null)
                    {
                        ticket.DiscountId = discount.DiscountId;
                        ticket.DiscountAmount = discountAmount;
                    }
                    
                    // Cập nhật trạng thái vé
                    ticket.Status = "DaThanhToan";
                }

                Console.WriteLine($"Updated {tickets.Count} tickets status to DaThanhToan");
                
                // Lưu vé trước
                Console.WriteLine("Saving ticket changes...");
                await _context.SaveChangesAsync();
                
                // Tạo Payment records cho các vé đã thanh toán (sử dụng raw SQL để tránh trigger conflict)
                foreach (var ticket in tickets)
                {
                    var sql = @"
                        INSERT INTO [ThanhToan] ([SoTien], [NgayThanhToan], [PhuongThucThanhToan], [MaVe])
                        VALUES (@amount, @paymentDate, @paymentMethod, @ticketId)";
                    
                    await _context.Database.ExecuteSqlRawAsync(sql,
                        new Microsoft.Data.SqlClient.SqlParameter("@amount", ticket.Price),
                        new Microsoft.Data.SqlClient.SqlParameter("@paymentDate", DateTime.Now),
                        new Microsoft.Data.SqlClient.SqlParameter("@paymentMethod", request.PaymentMethod),
                        new Microsoft.Data.SqlClient.SqlParameter("@ticketId", ticket.TicketId));
                    
                    Console.WriteLine($"Created payment for ticket {ticket.TicketId}, amount: {ticket.Price}");
                }
                
                Console.WriteLine("Committing transaction...");
                await transaction.CommitAsync();

                // Cập nhật doanh thu cho các suất chiếu
                var showtimeIds = tickets.Select(t => t.ShowtimeId).Distinct();
                foreach (var showtimeId in showtimeIds)
                {
                    await _revenueService.UpdateRevenueForShowtimeAsync(showtimeId);
                }

                // Tạo mã đặt vé
                var bookingCode = $"BK{DateTime.Now:yyyyMMdd}{tickets.First().TicketId:D6}";

                Console.WriteLine($"Payment successful! Booking code: {bookingCode}");

                return Json(new
                {
                    success = true,
                    message = "Thanh toán thành công!",
                    bookingCode = bookingCode,
                    ticketIds = tickets.Select(t => t.TicketId).ToList(),
                    updatedTickets = tickets.Count,
                    pricing = new
                    {
                        originalPrice = totalOriginalPrice,
                        discountAmount = discountAmount,
                        finalAmount = finalAmount,
                        discountCode = request.DiscountCode,
                        discountApplied = discountId.HasValue,
                        paymentAmountPerTicket = finalAmount / tickets.Count
                    }
                });
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi
                Console.WriteLine($"Error in ProcessSelectedPayments: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                string userMessage = "Có lỗi xảy ra khi thanh toán";
                
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                    {
                        userMessage = "Lỗi tham chiếu dữ liệu. Vui lòng kiểm tra lại thông tin.";
                    }
                    else if (ex.InnerException.Message.Contains("UNIQUE constraint"))
                    {
                        userMessage = "Dữ liệu đã tồn tại. Vui lòng thử lại.";
                    }
                    else if (ex.InnerException.Message.Contains("CHECK constraint"))
                    {
                        userMessage = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.InnerException.Message.Contains("Invalid column name"))
                    {
                        userMessage = "Lỗi cấu trúc database. Vui lòng liên hệ quản trị viên.";
                    }
                }
                
                return Json(new { success = false, message = userMessage, debugInfo = ex.InnerException?.Message });
            }
        }

        // Test action để kiểm tra tạo Payment đơn giản
        [HttpPost]
        public async Task<IActionResult> TestCreatePayment([FromBody] TestPaymentRequest request)
        {
            try
            {
                Console.WriteLine($"TestCreatePayment - TicketId: {request.TicketId}");
                Console.WriteLine($"TestCreatePayment - Amount: {request.Amount}");
                Console.WriteLine($"TestCreatePayment - PaymentMethod: {request.PaymentMethod}");

                // Tạo payment đơn giản
                var payment = new Payment
                {
                    TicketId = request.TicketId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentDate = DateTime.Now
                    // DiscountId = null // Tạm thời comment do cột chưa tồn tại
                };

                Console.WriteLine("Adding payment to context...");
                _context.Payments.Add(payment);
                
                Console.WriteLine("Saving changes...");
                await _context.SaveChangesAsync();

                Console.WriteLine("Payment created successfully!");

                return Json(new
                {
                    success = true,
                    message = "Test payment created successfully",
                    paymentId = payment.PaymentId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestCreatePayment Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                
                return Json(new
                {
                    success = false,
                    message = "Test payment failed",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // Debug action để kiểm tra việc áp dụng mã giảm giá
        [HttpPost]
        public async Task<IActionResult> DebugDiscountApplication([FromBody] DebugDiscountRequest request)
        {
            try
            {
                Console.WriteLine($"=== DebugDiscountApplication ===");
                Console.WriteLine($"Request: {System.Text.Json.JsonSerializer.Serialize(request)}");
                
                var userId = Request.Cookies["userId"];
                Console.WriteLine($"UserId: {userId}");
                
                // Lấy thông tin vé
                var tickets = await _context.Tickets
                    .Where(t => request.TicketIds.Contains(t.TicketId) && 
                               t.UserId.ToString() == userId && 
                               t.Status == "ChoXuLy")
                    .ToListAsync();
                
                Console.WriteLine($"Found {tickets.Count} tickets");
                
                if (!tickets.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy vé hợp lệ" });
                }
                
                // Tính giá gốc
                var totalOriginalPrice = tickets.Sum(t => t.Price);
                Console.WriteLine($"Total original price: {totalOriginalPrice}");
                
                // Tìm mã giảm giá
                Discount? discount = null;
                int? discountId = null;
                
                if (!string.IsNullOrEmpty(request.DiscountCode))
                {
                    discount = await _context.Discounts
                        .FirstOrDefaultAsync(d => d.Code == request.DiscountCode && d.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today));
                    
                    if (discount != null)
                    {
                        discountId = discount.DiscountId;
                        Console.WriteLine($"Found discount: {discount.Code}, Type: {discount.DiscountType}, Value: {discount.Value}");
                    }
                    else
                    {
                        Console.WriteLine($"Discount not found: {request.DiscountCode}");
                    }
                }
                
                // Tính toán giá sau khi giảm
                var finalAmount = totalOriginalPrice;
                var discountAmount = 0m;
                
                if (discountId.HasValue && discount != null)
                {
                    if (discount.DiscountType == "percentage")
                    {
                        discountAmount = totalOriginalPrice * (discount.Value / 100);
                        finalAmount = totalOriginalPrice - discountAmount;
                    }
                    else if (discount.DiscountType == "fixed")
                    {
                        discountAmount = discount.Value;
                        finalAmount = Math.Max(0, totalOriginalPrice - discountAmount);
                    }
                    
                    Console.WriteLine($"Discount calculation: {totalOriginalPrice} - {discountAmount} = {finalAmount}");
                }
                
                var result = new
                {
                    success = true,
                    message = "Debug discount application",
                    tickets = tickets.Select(t => new
                    {
                        t.TicketId,
                        t.Price,
                        t.Status
                    }).ToList(),
                    pricing = new
                    {
                        originalPrice = totalOriginalPrice,
                        discountAmount = discountAmount,
                        finalAmount = finalAmount,
                        discountCode = request.DiscountCode,
                        discountApplied = discountId.HasValue
                    },
                    discount = discount != null ? new
                    {
                        discount.DiscountId,
                        discount.Code,
                        discount.DiscountType,
                        discount.Value,
                        discount.ExpiryDate
                    } : null
                };
                
                Console.WriteLine($"Debug result: {System.Text.Json.JsonSerializer.Serialize(result)}");
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DebugDiscountApplication Error: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = "Lỗi khi debug mã giảm giá",
                    error = ex.Message
                });
            }
        }

        // Test action để kiểm tra mã giảm giá
        [HttpPost]
        public async Task<IActionResult> TestDiscount([FromBody] TestDiscountRequest request)
        {
            try
            {
                Console.WriteLine($"TestDiscount - Code: {request.DiscountCode}, SubTotal: {request.SubTotal}");
                
                var discount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.Code == request.DiscountCode && d.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today));
                
                if (discount == null)
                {
                    return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn" });
                }
                
                Console.WriteLine($"Found discount: {discount.Code}, Type: {discount.DiscountType}, Value: {discount.Value}");
                
                decimal discountAmount = 0;
                if (discount.DiscountType == "percentage")
                {
                    discountAmount = request.SubTotal * (discount.Value / 100);
                }
                else if (discount.DiscountType == "fixed")
                {
                    discountAmount = discount.Value;
                }
                
                var totalAmount = Math.Max(0, request.SubTotal - discountAmount);
                
                Console.WriteLine($"Discount calculation: {request.SubTotal} - {discountAmount} = {totalAmount}");
                
                return Json(new
                {
                    success = true,
                    message = "Mã giảm giá hợp lệ",
                    discount = new
                    {
                        code = discount.Code,
                        type = discount.DiscountType,
                        value = discount.Value
                    },
                    pricing = new
                    {
                        originalPrice = request.SubTotal,
                        discountAmount = discountAmount,
                        finalAmount = totalAmount
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestDiscount Error: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = "Lỗi khi kiểm tra mã giảm giá",
                    error = ex.Message
                });
            }
        }

        // Test action để cập nhật trạng thái vé
        [HttpPost]
        public async Task<IActionResult> TestUpdateTicketStatus([FromBody] TestUpdateStatusRequest request)
        {
            try
            {
                Console.WriteLine($"TestUpdateTicketStatus - TicketId: {request.TicketId}");
                
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketId == request.TicketId);
                
                if (ticket == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy vé" });
                }
                
                Console.WriteLine($"Current status: {ticket.Status}");
                
                // Cập nhật trạng thái
                ticket.Status = "DaThanhToan";
                
                Console.WriteLine("Saving changes...");
                await _context.SaveChangesAsync();
                
                Console.WriteLine("Status updated successfully");
                
                return Json(new { 
                    success = true, 
                    message = "Cập nhật trạng thái thành công",
                    ticketId = ticket.TicketId,
                    oldStatus = "ChoXuLy",
                    newStatus = ticket.Status
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestUpdateTicketStatus Error: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = "Lỗi khi cập nhật trạng thái",
                    error = ex.Message
                });
            }
        }

        // Debug action để kiểm tra thông tin mã giảm giá
        [HttpGet]
        public async Task<IActionResult> DebugDiscountInfo()
        {
            try
            {
                Console.WriteLine("=== DebugDiscountInfo ===");
                
                // Lấy tất cả mã giảm giá
                var discounts = await _context.Discounts.ToListAsync();
                
                Console.WriteLine($"Found {discounts.Count} discounts:");
                foreach (var discount in discounts)
                {
                    Console.WriteLine($"- Code: {discount.Code}, Type: {discount.DiscountType}, Value: {discount.Value}, Expiry: {discount.ExpiryDate}");
                }
                
                // Kiểm tra mã giam50k cụ thể
                var giam50k = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.Code == "giam50k");
                
                if (giam50k != null)
                {
                    Console.WriteLine($"giam50k details: Type={giam50k.DiscountType}, Value={giam50k.Value}, Expiry={giam50k.ExpiryDate}");
                    
                    // Test tính toán
                    var originalPrice = 95000m;
                    var discountAmount = 0m;
                    var finalAmount = originalPrice;
                    
                    if (giam50k.DiscountType == "percentage" || giam50k.DiscountType == "phantram")
                    {
                        discountAmount = originalPrice * (giam50k.Value / 100);
                        finalAmount = originalPrice - discountAmount;
                    }
                    else if (giam50k.DiscountType == "fixed" || giam50k.DiscountType == "codinh")
                    {
                        discountAmount = giam50k.Value;
                        finalAmount = Math.Max(0, originalPrice - discountAmount);
                    }
                    
                    Console.WriteLine($"Calculation test: {originalPrice} - {discountAmount} = {finalAmount}");
                }
                else
                {
                    Console.WriteLine("giam50k not found!");
                }
                
                return Json(new
                {
                    success = true,
                    message = "Debug discount info",
                    totalDiscounts = discounts.Count,
                    discounts = discounts.Select(d => new
                    {
                        d.Code,
                        d.DiscountType,
                        d.Value,
                        d.ExpiryDate
                    }).ToList(),
                    giam50k = giam50k != null ? new
                    {
                        giam50k.Code,
                        giam50k.DiscountType,
                        giam50k.Value,
                        giam50k.ExpiryDate
                    } : null
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DebugDiscountInfo Error: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = "Lỗi khi debug mã giảm giá",
                    error = ex.Message
                });
            }
        }

        // Test action đơn giản để kiểm tra mã giảm giá
        [HttpGet]
        public async Task<IActionResult> TestSimpleDiscount()
        {
            try
            {
                Console.WriteLine("=== TestSimpleDiscount ===");
                
                // Test tìm mã giảm giá
                var discount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.Code == "giam50k");
                
                if (discount == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy mã giảm giá giam50k" });
                }
                
                Console.WriteLine($"Found discount: {discount.Code}, Type: {discount.DiscountType}, Value: {discount.Value}");
                
                // Test tính toán giá
                var originalPrice = 95000m;
                var discountAmount = 0m;
                var finalAmount = originalPrice;
                
                if (discount.DiscountType == "percentage")
                {
                    discountAmount = originalPrice * (discount.Value / 100);
                    finalAmount = originalPrice - discountAmount;
                }
                else if (discount.DiscountType == "fixed")
                {
                    discountAmount = discount.Value;
                    finalAmount = Math.Max(0, originalPrice - discountAmount);
                }
                
                Console.WriteLine($"Price calculation: {originalPrice} - {discountAmount} = {finalAmount}");
                
                return Json(new
                {
                    success = true,
                    message = "Test mã giảm giá thành công",
                    discount = new
                    {
                        discount.DiscountId,
                        discount.Code,
                        discount.DiscountType,
                        discount.Value
                    },
                    pricing = new
                    {
                        originalPrice = originalPrice,
                        discountAmount = discountAmount,
                        finalAmount = finalAmount
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestSimpleDiscount Error: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = "Lỗi khi test mã giảm giá",
                    error = ex.Message
                });
            }
        }

        // Debug action để kiểm tra cấu trúc database
        [HttpGet]
        public async Task<IActionResult> DebugDatabase()
        {
            try
            {
                Console.WriteLine("=== DebugDatabase ===");
                
                // Test connection
                Console.WriteLine("Testing database connection...");
                var canConnect = await _context.Database.CanConnectAsync();
                Console.WriteLine($"Can connect: {canConnect}");

                // Kiểm tra dữ liệu mẫu
                Console.WriteLine("Getting sample tickets...");
                var sampleTickets = await _context.Tickets
                    .Where(t => t.Status == "ChoXuLy")
                    .Take(5)
                    .Select(t => new
                    {
                        t.TicketId,
                        t.UserId,
                        t.ShowtimeId,
                        t.SeatId,
                        t.Price,
                        t.Status
                    })
                    .ToListAsync();

                Console.WriteLine($"Found {sampleTickets.Count} pending tickets");

                Console.WriteLine("Getting sample discounts...");
                var sampleDiscounts = await _context.Discounts
                    .Take(5)
                    .Select(d => new
                    {
                        d.DiscountId,
                        d.Code,
                        d.DiscountType,
                        d.Value,
                        d.ExpiryDate
                    })
                    .ToListAsync();

                Console.WriteLine($"Found {sampleDiscounts.Count} discounts");

                var result = new
                {
                    success = true,
                    message = "Database debug info",
                    canConnect = canConnect,
                    sampleTickets = sampleTickets,
                    sampleDiscounts = sampleDiscounts,
                    ticketCount = await _context.Tickets.CountAsync(),
                    discountCount = await _context.Discounts.CountAsync()
                };

                Console.WriteLine($"Debug result: {System.Text.Json.JsonSerializer.Serialize(result)}");
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DebugDatabase Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                
                var errorResult = new
                {
                    success = false,
                    message = "Database debug failed",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                };
                
                Console.WriteLine($"Debug error result: {System.Text.Json.JsonSerializer.Serialize(errorResult)}");
                return Json(errorResult);
            }
        }
    }

    // Request models
    public class TestUpdateStatusRequest
    {
        public int TicketId { get; set; }
    }

    public class TestDiscountRequest
    {
        public string DiscountCode { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
    }

    public class DebugDiscountRequest
    {
        public List<int> TicketIds { get; set; } = new();
        public string DiscountCode { get; set; } = string.Empty;
    }

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

    public class PaymentSelectionRequest
    {
        public List<int> SelectedTicketIds { get; set; } = new();
        public string PaymentMethod { get; set; } = string.Empty;
        public string? DiscountCode { get; set; }
    }

    public class TestPaymentRequest
    {
        public int TicketId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
} 