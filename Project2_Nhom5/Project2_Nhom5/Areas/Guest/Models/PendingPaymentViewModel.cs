using System;
using System.Collections.Generic;

namespace Project2_Nhom5.Areas.Guest.Models
{
    public class PendingPaymentViewModel
    {
        public int TicketId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public string SeatCode { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PendingPaymentsViewModel
    {
        public List<PendingPaymentViewModel> PendingTickets { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string? DiscountCode { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class PaymentSelectionRequest
    {
        public List<int> SelectedTicketIds { get; set; } = new();
        public string PaymentMethod { get; set; } = string.Empty;
        public string? DiscountCode { get; set; }
    }
}
