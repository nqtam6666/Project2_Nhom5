using System;
using System.Collections.Generic;

namespace Project2_Nhom5.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string DiscountType { get; set; } = null!;

    public decimal Value { get; set; }

    public DateOnly ExpiryDate { get; set; }
}
