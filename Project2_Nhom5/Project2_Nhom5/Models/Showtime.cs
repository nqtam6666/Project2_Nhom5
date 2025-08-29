using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Project2_Nhom5.ModelBinders;

namespace Project2_Nhom5.Models;

public partial class Showtime
{
    public int ShowtimeId { get; set; }

    public int MovieId { get; set; }

    public int TheaterId { get; set; }

    [Required(ErrorMessage = "Ngày chiếu là bắt buộc")]
    [Display(Name = "Ngày chiếu")]
    public DateOnly ShowDate { get; set; }

    [Required(ErrorMessage = "Giờ chiếu là bắt buộc")]
    [Display(Name = "Giờ chiếu")]
    public TimeOnly ShowTime { get; set; }

    public virtual Movie? Movie { get; set; }

    public virtual ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();

    public virtual Theater? Theater { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
