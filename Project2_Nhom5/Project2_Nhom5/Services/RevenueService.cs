using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;

namespace Project2_Nhom5.Services
{
    public class RevenueService
    {
        private readonly Project2_Nhom5Context _context;

        public RevenueService(Project2_Nhom5Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Cập nhật doanh thu cho một suất chiếu
        /// </summary>
        /// <param name="showtimeId">ID của suất chiếu</param>
        /// <returns></returns>
        public async Task UpdateRevenueForShowtimeAsync(int showtimeId)
        {
            // Tính toán dữ liệu bán hàng thực tế
            var salesData = await _context.Tickets
                .Where(t => t.ShowtimeId == showtimeId && t.Status == "DaThanhToan")
                .Include(t => t.Payment)
                .GroupBy(t => t.ShowtimeId)
                .Select(g => new
                {
                    ShowtimeId = g.Key,
                    TicketsSold = g.Count(),
                    TotalTicketPrice = g.Sum(t => t.Price),
                    TotalPaymentAmount = g.Sum(t => t.Payment != null ? t.Payment.Amount : 0)
                })
                .FirstOrDefaultAsync();

            if (salesData == null)
            {
                return;
            }

            // Lấy tỷ lệ hoa hồng (mặc định 5% nếu chưa có)
            var existingRevenue = await _context.Revenues
                .FirstOrDefaultAsync(r => r.ShowtimeId == showtimeId);

            var commissionRate = existingRevenue?.AgencyCommission ?? 5.00m;

            // Cập nhật hoặc tạo bản ghi doanh thu
            if (existingRevenue != null)
            {
                existingRevenue.TotalAmount = salesData.TotalPaymentAmount;
                existingRevenue.TicketsSold = salesData.TicketsSold;
                existingRevenue.TotalTicketPrice = salesData.TotalTicketPrice;
                existingRevenue.AgencyCommission = commissionRate;
                // ActualRevenue is computed by database, don't set it manually
                
                _context.Revenues.Update(existingRevenue);
            }
            else
            {
                var newRevenue = new Revenue
                {
                    ShowtimeId = showtimeId,
                    TotalAmount = salesData.TotalPaymentAmount,
                    TicketsSold = salesData.TicketsSold,
                    TotalTicketPrice = salesData.TotalTicketPrice,
                    AgencyCommission = commissionRate,
                    // ActualRevenue is computed by database, don't set it manually
                    CreatedDate = DateTime.Now
                };

                _context.Revenues.Add(newRevenue);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lấy doanh thu thực tế cho một suất chiếu
        /// </summary>
        /// <param name="showtimeId">ID của suất chiếu</param>
        /// <returns></returns>
        public async Task<Revenue?> GetRevenueForShowtimeAsync(int showtimeId)
        {
            return await _context.Revenues
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Theater)
                .FirstOrDefaultAsync(r => r.ShowtimeId == showtimeId);
        }

        /// <summary>
        /// Lấy tất cả doanh thu với thông tin chi tiết
        /// </summary>
        /// <returns></returns>
        public async Task<List<Revenue>> GetAllRevenuesAsync()
        {
            return await _context.Revenues
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Theater)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        /// <summary>
        /// Tính tổng doanh thu trong khoảng thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns></returns>
        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Revenues
                .Where(r => r.CreatedDate >= startDate && r.CreatedDate <= endDate)
                .SumAsync(r => r.ActualRevenue ?? 0);
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo tháng
        /// </summary>
        /// <param name="year">Năm</param>
        /// <returns></returns>
        public async Task<List<object>> GetMonthlyRevenueStatsAsync(int year)
        {
            return await _context.Revenues
                .Where(r => r.CreatedDate.HasValue && r.CreatedDate.Value.Year == year)
                .GroupBy(r => r.CreatedDate.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(r => r.ActualRevenue ?? 0),
                    TotalTickets = g.Sum(r => r.TicketsSold ?? 0),
                    ShowtimeCount = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToListAsync<object>();
        }
    }
}
