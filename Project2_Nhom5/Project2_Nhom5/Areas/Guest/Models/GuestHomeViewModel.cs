using System.Collections.Generic;
using Project2_Nhom5.Models;

namespace Project2_Nhom5.Areas.Guest.Models
{
    public class GuestHomeViewModel
    {
        public List<Movie> NowShowing { get; set; } = new();
        public List<Movie> ComingSoon { get; set; } = new();
        public List<Movie> Stopped { get; set; } = new();
    }
} 