using System.ComponentModel.DataAnnotations;

namespace Project2_Nhom5.Areas.Admin.Models
{
    public class BulkSeatCreateModel
    {
        [Required(ErrorMessage = "Vui lòng chọn rạp chiếu")]
        [Display(Name = "Rạp chiếu")]
        public int TheaterId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại ghế")]
        [Display(Name = "Loại ghế")]
        public string SeatType { get; set; } = "thuong";

        [Required(ErrorMessage = "Vui lòng nhập số hàng bắt đầu")]
        [Range(1, 26, ErrorMessage = "Số hàng phải từ 1-26 (A-Z)")]
        [Display(Name = "Hàng bắt đầu")]
        public int StartRow { get; set; } = 1;

        [Required(ErrorMessage = "Vui lòng nhập số hàng kết thúc")]
        [Range(1, 26, ErrorMessage = "Số hàng phải từ 1-26 (A-Z)")]
        [Display(Name = "Hàng kết thúc")]
        public int EndRow { get; set; } = 10;

        [Required(ErrorMessage = "Vui lòng nhập số cột bắt đầu")]
        [Range(1, 50, ErrorMessage = "Số cột phải từ 1-50")]
        [Display(Name = "Cột bắt đầu")]
        public int StartColumn { get; set; } = 1;

        [Required(ErrorMessage = "Vui lòng nhập số cột kết thúc")]
        [Range(1, 50, ErrorMessage = "Số cột phải từ 1-50")]
        [Display(Name = "Cột kết thúc")]
        public int EndColumn { get; set; } = 15;

        [Display(Name = "Bỏ qua ghế đã tồn tại")]
        public bool SkipExisting { get; set; } = true;
    }
} 