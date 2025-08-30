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
    public class TheatersController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public TheatersController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Theaters
        public async Task<IActionResult> Index(string search, string location, string sortOrder, string currentSort, int page = 1, int pageSize = 10)
        {
            // Get all theaters
            var theaters = _context.Theaters.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                theaters = theaters.Where(t => 
                    t.Name.Contains(search) || 
                    t.Location.Contains(search) ||
                    t.RoomNumber.ToString().Contains(search)
                );
            }

            // Apply location filter
            if (!string.IsNullOrEmpty(location))
            {
                theaters = theaters.Where(t => t.Location == location);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(currentSort))
            {
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            }

            theaters = currentSort switch
            {
                "name" => sortOrder == "desc" ? theaters.OrderByDescending(t => t.Name) : theaters.OrderBy(t => t.Name),
                "location" => sortOrder == "desc" ? theaters.OrderByDescending(t => t.Location) : theaters.OrderBy(t => t.Location),
                "roomNumber" => sortOrder == "desc" ? theaters.OrderByDescending(t => t.RoomNumber) : theaters.OrderBy(t => t.RoomNumber),
                _ => theaters.OrderByDescending(t => t.TheaterId)
            };

            // Get total count for pagination
            var totalCount = await theaters.CountAsync();

            // Apply pagination
            var skip = (page - 1) * pageSize;
            var pagedTheaters = await theaters.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;
            ViewBag.Location = location;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentSort = currentSort;

            // Get unique locations for filter dropdown
            ViewBag.Locations = await _context.Theaters
                .Select(t => t.Location)
                .Distinct()
                .OrderBy(l => l)
                .ToListAsync();

            return View(pagedTheaters);
        }

        // GET: Theaters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theater = await _context.Theaters
                .FirstOrDefaultAsync(m => m.TheaterId == id);
            if (theater == null)
            {
                return NotFound();
            }

            return View(theater);
        }

        // GET: Theaters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Theaters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TheaterId,Name,Location,RoomNumber")] Theater theater)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(theater);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Tạo rạp chiếu thành công!" });
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
                    string userMessage = "Có lỗi xảy ra khi tạo rạp chiếu";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa rạp này vì đã có lịch chiếu hoặc ghế được tạo. Vui lòng xóa lịch chiếu và ghế trước.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Tên rạp hoặc địa điểm này đã tồn tại. Vui lòng chọn tên khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return View(theater);
        }

        // GET: Theaters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theater = await _context.Theaters.FindAsync(id);
            if (theater == null)
            {
                return NotFound();
            }
            return View(theater);
        }

        // POST: Theaters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TheaterId,Name,Location,RoomNumber")] Theater theater)
        {
            try
            {
                if (id != theater.TheaterId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(theater);
                        await _context.SaveChangesAsync();
                        
                        // Check if request wants JSON response
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = true, message = "Cập nhật rạp chiếu thành công!" });
                        }
                        
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TheaterExists(theater.TheaterId))
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
                    string userMessage = "Có lỗi xảy ra khi cập nhật rạp chiếu";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể cập nhật rạp này vì đã có lịch chiếu hoặc ghế được tạo. Vui lòng xóa lịch chiếu và ghế trước.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Tên rạp hoặc địa điểm này đã tồn tại. Vui lòng chọn tên khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return View(theater);
        }

        // GET: Theaters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theater = await _context.Theaters
                .FirstOrDefaultAsync(m => m.TheaterId == id);
            if (theater == null)
            {
                return NotFound();
            }

            return View(theater);
        }

        // POST: Theaters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var theater = await _context.Theaters.FindAsync(id);
                if (theater != null)
                {
                    _context.Theaters.Remove(theater);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Xóa rạp chiếu thành công!" });
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy rạp chiếu để xóa" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi xóa rạp chiếu";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa rạp chiếu vì đã có lịch chiếu hoặc ghế được tạo. Vui lòng xóa lịch chiếu và ghế trước.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool TheaterExists(int id)
        {
            return _context.Theaters.Any(e => e.TheaterId == id);
        }
    }
}
