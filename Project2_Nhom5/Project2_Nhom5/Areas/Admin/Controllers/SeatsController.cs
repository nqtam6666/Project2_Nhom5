using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;
using Project2_Nhom5.Areas.Admin.Models;

namespace Project2_Nhom5.Controllers
{
    [Area("Admin")]
    public class SeatsController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public SeatsController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Seats
        public async Task<IActionResult> Index(string search, string theaterId, string seatType, string sortOrder, string currentSort, int page = 1, int pageSize = 10)
        {
            // Get all seats with includes
            var seats = _context.Seats
                .Include(s => s.Theater)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                seats = seats.Where(s => 
                    s.SeatCode.Contains(search) || 
                    s.SeatType.Contains(search) ||
                    s.Theater.Name.Contains(search) ||
                    s.Theater.Location.Contains(search)
                );
            }

            // Apply theater filter
            if (!string.IsNullOrEmpty(theaterId) && int.TryParse(theaterId, out int theaterIdInt))
            {
                seats = seats.Where(s => s.TheaterId == theaterIdInt);
            }

            // Apply seat type filter
            if (!string.IsNullOrEmpty(seatType))
            {
                seats = seats.Where(s => s.SeatType == seatType);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(currentSort))
            {
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            }

            seats = currentSort switch
            {
                "seatCode" => sortOrder == "desc" ? seats.OrderByDescending(s => s.SeatCode) : seats.OrderBy(s => s.SeatCode),
                "seatType" => sortOrder == "desc" ? seats.OrderByDescending(s => s.SeatType) : seats.OrderBy(s => s.SeatType),
                "theater" => sortOrder == "desc" ? seats.OrderByDescending(s => s.Theater.Name) : seats.OrderBy(s => s.Theater.Name),
                _ => seats.OrderByDescending(s => s.SeatId)
            };

            // Get total count for pagination
            var totalCount = await seats.CountAsync();

            // Apply pagination
            var skip = (page - 1) * pageSize;
            var pagedSeats = await seats.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;
            ViewBag.TheaterId = theaterId;
            ViewBag.SeatType = seatType;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentSort = currentSort;

            // Get theaters for filter dropdown
            ViewBag.Theaters = await _context.Theaters.OrderBy(t => t.Name).ToListAsync();

            return View(pagedSeats);
        }

        // GET: Seats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seat = await _context.Seats
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(m => m.SeatId == id);
            if (seat == null)
            {
                return NotFound();
            }

            return View(seat);
        }

        // GET: Seats/Create
        public IActionResult Create()
        {
            ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "TheaterId");
            return View();
        }

        // POST: Seats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeatId,TheaterId,SeatCode,SeatType")] Seat seat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(seat);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo ghế ngồi thành công!" });
                    }
                    
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        return Json(new { success = false, message = string.Join(", ", errors) });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi tạo ghế ngồi";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa ghế này vì đã có vé được đặt cho ghế này. Vui lòng hủy vé trước khi xóa ghế.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Mã ghế này đã tồn tại trong rạp. Vui lòng chọn mã khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "TheaterId", seat.TheaterId);
            return View(seat);
        }

        // GET: Seats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seat = await _context.Seats.FindAsync(id);
            if (seat == null)
            {
                return NotFound();
            }
            ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "TheaterId", seat.TheaterId);
            return View(seat);
        }

        // POST: Seats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SeatId,TheaterId,SeatCode,SeatType")] Seat seat)
        {
            try
            {
                if (id != seat.SeatId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(seat);
                        await _context.SaveChangesAsync();
                        
                        // Check if request wants JSON response
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = true, message = "Cập nhật ghế ngồi thành công!" });
                        }
                        
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SeatExists(seat.SeatId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        return Json(new { success = false, message = string.Join(", ", errors) });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi cập nhật ghế ngồi";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể cập nhật ghế này vì đã có vé được đặt. Vui lòng hủy vé trước khi thay đổi.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Mã ghế này đã tồn tại trong rạp. Vui lòng chọn mã khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "TheaterId", seat.TheaterId);
            return View(seat);
        }

        // GET: Seats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seat = await _context.Seats
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(m => m.SeatId == id);
            if (seat == null)
            {
                return NotFound();
            }

            return View(seat);
        }

        // POST: Seats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var seat = await _context.Seats.FindAsync(id);
                if (seat != null)
                {
                    _context.Seats.Remove(seat);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Xóa ghế ngồi thành công!" });
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy ghế ngồi để xóa" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi xóa ghế ngồi";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa ghế này vì đã có vé được đặt. Vui lòng hủy vé trước.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool SeatExists(int id)
        {
            return _context.Seats.Any(e => e.SeatId == id);
        }

        // GET: Seats/BulkCreate
        public async Task<IActionResult> BulkCreate()
        {
            ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "Name");
            return View();
        }

        // POST: Seats/BulkCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkCreate([Bind("TheaterId,SeatType,StartRow,EndRow,StartColumn,EndColumn,SkipExisting")] BulkSeatCreateModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate ranges
                    if (model.StartRow > model.EndRow)
                    {
                        ModelState.AddModelError("EndRow", "Hàng kết thúc phải lớn hơn hoặc bằng hàng bắt đầu");
                        ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "Name", model.TheaterId);
                        return View(model);
                    }

                    if (model.StartColumn > model.EndColumn)
                    {
                        ModelState.AddModelError("EndColumn", "Cột kết thúc phải lớn hơn hoặc bằng cột bắt đầu");
                        ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "Name", model.TheaterId);
                        return View(model);
                    }

                    var seatsToAdd = new List<Seat>();
                    var existingSeats = new List<string>();
                    var addedSeats = new List<string>();

                    // Generate seats
                    for (int row = model.StartRow; row <= model.EndRow; row++)
                    {
                        for (int col = model.StartColumn; col <= model.EndColumn; col++)
                        {
                            var rowLetter = ((char)('A' + row - 1)).ToString();
                            var seatCode = $"{rowLetter}{col:D2}";

                            // Check if seat already exists
                            var existingSeat = await _context.Seats
                                .FirstOrDefaultAsync(s => s.TheaterId == model.TheaterId && s.SeatCode == seatCode);

                            if (existingSeat != null)
                            {
                                if (model.SkipExisting)
                                {
                                    existingSeats.Add(seatCode);
                                    continue;
                                }
                                else
                                {
                                    ModelState.AddModelError("", $"Ghế {seatCode} đã tồn tại trong rạp này");
                                    ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "Name", model.TheaterId);
                                    return View(model);
                                }
                            }

                            var newSeat = new Seat
                            {
                                TheaterId = model.TheaterId,
                                SeatCode = seatCode,
                                SeatType = model.SeatType
                            };

                            seatsToAdd.Add(newSeat);
                            addedSeats.Add(seatCode);
                        }
                    }

                    if (seatsToAdd.Count > 0)
                    {
                        await _context.Seats.AddRangeAsync(seatsToAdd);
                        await _context.SaveChangesAsync();
                    }

                    // Prepare response message
                    var message = $"Đã tạo thành công {addedSeats.Count} ghế";
                    if (existingSeats.Count > 0)
                    {
                        message += $", bỏ qua {existingSeats.Count} ghế đã tồn tại";
                    }

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { 
                            success = true, 
                            message = message,
                            addedCount = addedSeats.Count,
                            skippedCount = existingSeats.Count,
                            addedSeats = addedSeats,
                            skippedSeats = existingSeats
                        });
                    }

                    TempData["SuccessMessage"] = message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        return Json(new { success = false, message = string.Join(", ", errors) });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi tạo ghế hàng loạt: " + ex.Message });
                }
            }

            ViewData["TheaterId"] = new SelectList(_context.Theaters, "TheaterId", "Name", model.TheaterId);
            return View(model);
        }
    }
}
