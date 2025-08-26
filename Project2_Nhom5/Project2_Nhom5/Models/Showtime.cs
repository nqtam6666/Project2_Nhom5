using System;
using System.Collections.Generic;

namespace Project2_Nhom5.Models;

public partial class Showtime
{
    public int ShowtimeId { get; set; }

    public int MovieId { get; set; }

    public int TheaterId { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly ShowTime { get; set; }

    public virtual Movie? Movie { get; set; }

    public virtual ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();

    public virtual Theater? Theater { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
