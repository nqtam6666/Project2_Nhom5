using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;

namespace Project2_Nhom5.Controllers
{
    [Area("Admin")]
    public class DiscountsController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public DiscountsController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Discounts
        public async Task<IActionResult> Index(string search, string type, string expiryStatus, string sortOrder, string currentSort, int page = 1, int pageSize = 10)
        {
            // Get all discounts
            var discounts = _context.Discounts.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                discounts = discounts.Where(d => 
                    d.Code.Contains(search) || 
                    d.Description.Contains(search)
                );
            }

            // Apply type filter
            if (!string.IsNullOrEmpty(type))
            {
                discounts = discounts.Where(d => d.DiscountType == type);
            }

            // Apply expiry status filter
            if (!string.IsNullOrEmpty(expiryStatus))
            {
                var today = DateTime.Today;
                if (expiryStatus == "active")
                {
                    discounts = discounts.Where(d => d.ExpiryDate.ToDateTime(TimeOnly.MinValue) >= today);
                }
                else if (expiryStatus == "expired")
                {
                    discounts = discounts.Where(d => d.ExpiryDate.ToDateTime(TimeOnly.MinValue) < today);
                }
                else if (expiryStatus == "expiring")
                {
                    var sevenDaysFromNow = today.AddDays(7);
                    discounts = discounts.Where(d => d.ExpiryDate.ToDateTime(TimeOnly.MinValue) >= today && d.ExpiryDate.ToDateTime(TimeOnly.MinValue) < sevenDaysFromNow);
                }
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(currentSort))
            {
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            }

            discounts = currentSort switch
            {
                "code" => sortOrder == "desc" ? discounts.OrderByDescending(d => d.Code) : discounts.OrderBy(d => d.Code),
                "type" => sortOrder == "desc" ? discounts.OrderByDescending(d => d.DiscountType) : discounts.OrderBy(d => d.DiscountType),
                "value" => sortOrder == "desc" ? discounts.OrderByDescending(d => d.Value) : discounts.OrderBy(d => d.Value),
                "expiry" => sortOrder == "desc" ? discounts.OrderByDescending(d => d.ExpiryDate) : discounts.OrderBy(d => d.ExpiryDate),
                _ => discounts.OrderByDescending(d => d.DiscountId)
            };

            // Get total count for pagination
            var totalCount = await discounts.CountAsync();

            // Apply pagination
            var skip = (page - 1) * pageSize;
            var pagedDiscounts = await discounts.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;
            ViewBag.Type = type;
            ViewBag.ExpiryStatus = expiryStatus;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentSort = currentSort;

            return View(pagedDiscounts);
        }

        // GET: Discounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts
                .FirstOrDefaultAsync(m => m.DiscountId == id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // GET: Discounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiscountId,Code,Description,DiscountType,Value,ExpiryDate")] Discount discount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(discount);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo mã giảm giá thành công!" });
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
                    string userMessage = "Có lỗi xảy ra khi tạo mã giảm giá";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể tạo mã giảm giá vì có lỗi ràng buộc dữ liệu. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Mã giảm giá này đã tồn tại. Vui lòng chọn mã khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return View(discount);
        }

        // GET: Discounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        // POST: Discounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiscountId,Code,Description,DiscountType,Value,ExpiryDate")] Discount discount)
        {
            try
            {
                if (id != discount.DiscountId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(discount);
                        await _context.SaveChangesAsync();
                        
                        // Check if request wants JSON response
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = true, message = "Cập nhật mã giảm giá thành công!" });
                        }
                        
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DiscountExists(discount.DiscountId))
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
                    string userMessage = "Có lỗi xảy ra khi cập nhật mã giảm giá";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể cập nhật mã giảm giá vì có lỗi ràng buộc dữ liệu. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Mã giảm giá này đã tồn tại. Vui lòng chọn mã khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return View(discount);
        }

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts
                .FirstOrDefaultAsync(m => m.DiscountId == id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // POST: Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(id);
                if (discount != null)
                {
                    _context.Discounts.Remove(discount);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Xóa mã giảm giá thành công!" });
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy mã giảm giá để xóa" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi xóa mã giảm giá";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa mã giảm giá vì có dữ liệu liên quan. Vui lòng kiểm tra lại.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountExists(int id)
        {
            return _context.Discounts.Any(e => e.DiscountId == id);
        }
    }
}
