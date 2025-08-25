using System;
using System.Collections.Generic;

namespace Project2_Nhom5.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int? TheaterId { get; set; }

    public string SeatCode { get; set; } = null!;

    public string? SeatType { get; set; }

    public virtual Theater? Theater { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
