using System;
using System.Collections.Generic;

namespace Project2_Nhom5.Models;

public partial class Theater
{
    public int TheaterId { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public int RoomNumber { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
