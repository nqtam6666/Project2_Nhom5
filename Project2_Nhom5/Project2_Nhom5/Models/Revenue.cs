using System;
using System.Collections.Generic;

namespace Project2_Nhom5.Models;

public partial class Revenue
{
    public int RevenueId { get; set; }

    public int? ShowtimeId { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? AgencyCommission { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? TicketsSold { get; set; }

    public decimal? TotalTicketPrice { get; set; }

    public decimal? ActualRevenue { get; set; }

    public virtual Showtime? Showtime { get; set; }
}
