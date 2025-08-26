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
    public class UsersController : Controller
    {
        private readonly Project2_Nhom5Context _context;

        public UsersController(Project2_Nhom5Context context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Tickets)
                .ThenInclude(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Password,Email,Phone,Role,Status")] User user)
        {
            // Check for duplicate email
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
            }

            // Check for duplicate username
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Handle any other database constraint violations
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.");
                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Convert to EditUserViewModel
            var editViewModel = new EditUserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Status = user.Status
                // Password is intentionally left null for edit
            };

            return View(editViewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,Email,Phone,Role,Status")] EditUserViewModel editViewModel)
        {
            if (id != editViewModel.UserId)
            {
                return NotFound();
            }

            // Get the original user to preserve password if not changed
            var originalUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
            if (originalUser == null)
            {
                return NotFound();
            }

            // Check for duplicate email (excluding current user)
            if (await _context.Users.AnyAsync(u => u.Email == editViewModel.Email && u.UserId != id))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
            }

            // Check for duplicate username (excluding current user)
            if (await _context.Users.AnyAsync(u => u.Username == editViewModel.Username && u.UserId != id))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create a new User entity with the updated data
                    var user = new User
                    {
                        UserId = editViewModel.UserId,
                        Username = editViewModel.Username,
                        Email = editViewModel.Email,
                        Phone = editViewModel.Phone,
                        Role = editViewModel.Role,
                        Status = editViewModel.Status,
                        // Handle password: if empty, keep the original password
                        Password = string.IsNullOrWhiteSpace(editViewModel.Password) 
                            ? originalUser.Password 
                            : editViewModel.Password
                    };

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(editViewModel.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Handle any other database constraint violations
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.");
                    return View(editViewModel);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editViewModel);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Tickets)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        // AJAX methods for client-side validation
        [HttpGet]
        public async Task<IActionResult> CheckEmailExists(string email, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(false);

            var query = _context.Users.Where(u => u.Email == email);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserId != excludeUserId.Value);
            }

            var exists = await query.AnyAsync();
            return Json(exists);
        }

        [HttpGet]
        public async Task<IActionResult> CheckUsernameExists(string username, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Json(false);

            var query = _context.Users.Where(u => u.Username == username);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserId != excludeUserId.Value);
            }

            var exists = await query.AnyAsync();
            return Json(exists);
        }
    }
}
