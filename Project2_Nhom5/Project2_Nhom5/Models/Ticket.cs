using System;
using System.Collections.Generic;

namespace Project2_Nhom5.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int? UserId { get; set; }

    public int? ShowtimeId { get; set; }

    public int? SeatId { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Seat? Seat { get; set; }

    public virtual Showtime? Showtime { get; set; }

    public virtual User? User { get; set; }
}
