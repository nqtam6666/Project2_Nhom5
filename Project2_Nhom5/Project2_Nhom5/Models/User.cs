using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project2_Nhom5.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự và không quá 255 ký tự")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    public string Email { get; set; } = null!;

    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
    public string? Phone { get; set; }

    [StringLength(50, ErrorMessage = "Vai trò không được vượt quá 50 ký tự")]
    [RegularExpression("^(NguoiDung|Admin|DaiLy)$", ErrorMessage = "Vai trò phải là một trong các giá trị: NguoiDung, Admin, DaiLy")]
    public string? Role { get; set; }

    [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
    [RegularExpression("^(hoatdong|khonghoatdong)$", ErrorMessage = "Trạng thái phải là một trong các giá trị: hoatdong, khonghoatdong")]
    public string? Status { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

// View model for editing users (allows optional password)
public class EditUserViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
    public string Username { get; set; } = null!;

    [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự và không quá 255 ký tự")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    public string Email { get; set; } = null!;

    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
    public string? Phone { get; set; }

    [StringLength(50, ErrorMessage = "Vai trò không được vượt quá 50 ký tự")]
    [RegularExpression("^(NguoiDung|Admin|DaiLy)$", ErrorMessage = "Vai trò phải là một trong các giá trị: NguoiDung, Admin, DaiLy")]
    public string? Role { get; set; }

    [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
    [RegularExpression("^(hoatdong|khonghoatdong)$", ErrorMessage = "Trạng thái phải là một trong các giá trị: hoatdong, khonghoatdong")]
    public string? Status { get; set; }
}
