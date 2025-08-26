using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Project2_Nhom5.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly Project2_Nhom5Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MoviesController(Project2_Nhom5Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Genre,Duration,Description,PosterUrl,TrailerUrl,Status")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            
            // Create genre list for dropdown
            var genres = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Chọn thể loại..." },
                new SelectListItem { Value = "Hành động", Text = "Hành động" },
                new SelectListItem { Value = "Tình cảm", Text = "Tình cảm" },
                new SelectListItem { Value = "Hài hước", Text = "Hài hước" },
                new SelectListItem { Value = "Kinh dị", Text = "Kinh dị" },
                new SelectListItem { Value = "Viễn tưởng", Text = "Viễn tưởng" },
                new SelectListItem { Value = "Hoạt hình", Text = "Hoạt hình" },
                new SelectListItem { Value = "Tài liệu", Text = "Tài liệu" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };
            
            ViewData["Genres"] = new SelectList(genres, "Value", "Text", movie.Genre);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,Genre,Duration,Description,PosterUrl,TrailerUrl,Status")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            // Recreate genre list for validation errors
            var genres = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Chọn thể loại..." },
                new SelectListItem { Value = "Hành động", Text = "Hành động" },
                new SelectListItem { Value = "Tình cảm", Text = "Tình cảm" },
                new SelectListItem { Value = "Hài hước", Text = "Hài hước" },
                new SelectListItem { Value = "Kinh dị", Text = "Kinh dị" },
                new SelectListItem { Value = "Viễn tưởng", Text = "Viễn tưởng" },
                new SelectListItem { Value = "Hoạt hình", Text = "Hoạt hình" },
                new SelectListItem { Value = "Tài liệu", Text = "Tài liệu" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };
            
            ViewData["Genres"] = new SelectList(genres, "Value", "Text", movie.Genre);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }

        // POST: Movies/UploadPoster
        [HttpPost]
        public async Task<IActionResult> UploadPoster(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn file để upload." });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Chỉ chấp nhận file hình ảnh: JPG, PNG, GIF, WebP." });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File quá lớn. Kích thước tối đa là 5MB." });
                }

                // Generate unique filename
                var fileName = $"poster_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{fileExtension}";
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                
                // Ensure directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);
                
                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path for database
                var relativePath = $"img/{fileName}";
                
                return Json(new { 
                    success = true, 
                    message = "Upload thành công!", 
                    path = relativePath,
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi upload: {ex.Message}" });
            }
        }
    }
}
