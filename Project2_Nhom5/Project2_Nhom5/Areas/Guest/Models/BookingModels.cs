using System.ComponentModel.DataAnnotations;

namespace Project2_Nhom5.Areas.Guest.Models
{
    // View model cho việc chọn suất chiếu
    public class ShowtimeSelectionViewModel
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public string MoviePoster { get; set; } = string.Empty;
        public decimal BasePrice { get; set; } = 75000; // Giá vé cơ bản
    }

    // View model cho việc chọn ghế
    public class SeatSelectionViewModel
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public string MoviePoster { get; set; } = string.Empty;
        public List<SeatViewModel> AvailableSeats { get; set; } = new();
        public List<SeatViewModel> BookedSeats { get; set; } = new();
        public List<int> SelectedSeatIds { get; set; } = new();
        public decimal BasePrice { get; set; } = 75000;
    }

    // View model cho ghế
    public class SeatViewModel
    {
        public int SeatId { get; set; }
        public string SeatCode { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty; // thuong, VIP
        public bool IsBooked { get; set; }
        public bool IsSelected { get; set; }
        public decimal Price { get; set; }
        public string Row { get; set; } = string.Empty; // A, B, C,...
        public int Column { get; set; } // 1, 2, 3,...
    }

    // View model cho thanh toán
    public class PaymentViewModel
    {
        public int ShowtimeId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public string MoviePoster { get; set; } = string.Empty;
        public List<SeatViewModel> SelectedSeats { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Mã giảm giá")]
        public string? DiscountCode { get; set; }
        
        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
    }

    // View model cho xác nhận đặt vé
    public class BookingConfirmationViewModel
    {
        public int TicketId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public List<string> SeatCodes { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string BookingCode { get; set; } = string.Empty;
    }

    // View model cho lịch sử đặt vé
    public class BookingHistoryViewModel
    {
        public int TicketId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public List<string> SeatCodes { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string? PaymentMethod { get; set; }
    }
} 