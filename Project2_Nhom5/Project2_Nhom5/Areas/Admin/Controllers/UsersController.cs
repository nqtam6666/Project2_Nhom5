using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;
using Project2_Nhom5.Services;

namespace Project2_Nhom5.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly Project2_Nhom5Context _context;
        private readonly IAuthService _authService;

        public UsersController(Project2_Nhom5Context context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET: Users
        public async Task<IActionResult> Index(string search, string role, string status, string sortOrder, string currentSort, int page = 1, int pageSize = 10)
        {
            // Get all users
            var users = _context.Users.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => 
                    u.Username.Contains(search) || 
                    u.Email.Contains(search) || 
                    u.Phone.Contains(search)
                );
            }

            // Apply role filter
            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role == role);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                users = users.Where(u => u.Status == status);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(currentSort))
            {
                sortOrder = sortOrder == "asc" ? "desc" : "asc";
            }

            users = currentSort switch
            {
                "username" => sortOrder == "desc" ? users.OrderByDescending(u => u.Username) : users.OrderBy(u => u.Username),
                "email" => sortOrder == "desc" ? users.OrderByDescending(u => u.Email) : users.OrderBy(u => u.Email),
                "role" => sortOrder == "desc" ? users.OrderByDescending(u => u.Role) : users.OrderBy(u => u.Role),
                "status" => sortOrder == "desc" ? users.OrderByDescending(u => u.Status) : users.OrderBy(u => u.Status),
                _ => users.OrderByDescending(u => u.UserId)
            };

            // Get total count for pagination
            var totalCount = await users.CountAsync();

            // Apply pagination
            var skip = (page - 1) * pageSize;
            var pagedUsers = await users.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.Status = status;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentSort = currentSort;

            return View(pagedUsers);
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
            try
            {
                // Log the incoming data for debugging
                Console.WriteLine($"Creating user: Username={user.Username}, Email={user.Email}, Role={user.Role}, Status={user.Status}");
                
                // Check for duplicate email and username only if model is valid
                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid, checking for duplicates...");
                    
                    // Check for duplicate email
                    var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                    if (existingEmail != null)
                    {
                        Console.WriteLine($"Duplicate email found: {user.Email}");
                        ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
                    }

                    // Check for duplicate username
                    var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                    if (existingUsername != null)
                    {
                        Console.WriteLine($"Duplicate username found: {user.Username}");
                        ModelState.AddModelError("Username", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.");
                    }
                }
                else
                {
                    Console.WriteLine("ModelState is invalid:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"  - {error.ErrorMessage}");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        Console.WriteLine("Attempting to save user to database...");
                        
                        // Set default values if not provided
                        if (string.IsNullOrEmpty(user.Role))
                        {
                            user.Role = "NguoiDung";
                        }
                        if (string.IsNullOrEmpty(user.Status))
                        {
                            user.Status = "hoatdong";
                        }

                        // Hash password using AuthService
                        user.Password = _authService.HashPassword(user.Password);
                        Console.WriteLine("Password hashed successfully");

                        _context.Add(user);
                        await _context.SaveChangesAsync();
                        
                        Console.WriteLine($"User created successfully with ID: {user.UserId}");
                        
                        // Check if request wants JSON response
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = true, message = "Tạo người dùng thành công!" });
                        }
                        
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Database error: {ex.Message}");
                        Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                        
                        // Handle any other database constraint violations
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            string userMessage = "Có lỗi xảy ra khi lưu dữ liệu";
                            
                            if (ex.InnerException?.Message?.Contains("duplicate") == true)
                            {
                                userMessage = "Email hoặc tên đăng nhập này đã tồn tại. Vui lòng chọn thông tin khác.";
                            }
                            else if (ex.InnerException?.Message?.Contains("CHECK constraint") == true)
                            {
                                userMessage = "Giá trị vai trò hoặc trạng thái không hợp lệ. Vui lòng kiểm tra lại.";
                            }
                            else
                            {
                                // Log the actual error for debugging
                                Console.WriteLine($"User creation error: {ex.Message}");
                                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                                userMessage = "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.";
                            }
                            
                            return Json(new { success = false, message = userMessage });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.");
                        }
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        var errorMessage = string.Join(", ", errors);
                        Console.WriteLine($"Validation errors: {errorMessage}");
                        return Json(new { success = false, message = errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi tạo người dùng";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể tạo người dùng vì có lỗi ràng buộc dữ liệu. Vui lòng kiểm tra lại.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Email hoặc tên đăng nhập này đã tồn tại. Vui lòng chọn thông tin khác.";
                    }
                    else if (ex.InnerException?.Message?.Contains("duplicate") == true)
                    {
                        userMessage = "Email hoặc tên đăng nhập này đã tồn tại. Vui lòng chọn thông tin khác.";
                    }
                    else if (ex.InnerException?.Message?.Contains("CHECK constraint") == true)
                    {
                        userMessage = "Giá trị vai trò hoặc trạng thái không hợp lệ. Vui lòng kiểm tra lại.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo người dùng. Vui lòng kiểm tra lại thông tin.");
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
            try
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
                            // Handle password: if empty, keep the original password, otherwise hash the new password
                            Password = string.IsNullOrWhiteSpace(editViewModel.Password) 
                                ? originalUser.Password 
                                : _authService.HashPassword(editViewModel.Password)
                        };

                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        
                        // Check if request wants JSON response
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = true, message = "Cập nhật người dùng thành công!" });
                        }
                        
                        return RedirectToAction(nameof(Index));
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
                    catch
                    {
                        // Handle any other database constraint violations
                        ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.");
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin." });
                        }
                        return View(editViewModel);
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
                    string userMessage = "Có lỗi xảy ra khi cập nhật người dùng";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể cập nhật người dùng này vì đã có vé được đặt. Vui lòng hủy vé trước.";
                    }
                    else if (ex.Message.Contains("duplicate") || ex.Message.Contains("trùng"))
                    {
                        userMessage = "Email hoặc tên đăng nhập này đã tồn tại. Vui lòng chọn thông tin khác.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
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
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    
                    // Check if request wants JSON response
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Xóa người dùng thành công!" });
                    }
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Không tìm thấy người dùng để xóa" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    string userMessage = "Có lỗi xảy ra khi xóa người dùng";
                    
                    if (ex.InnerException?.Message?.Contains("REFERENCE constraint") == true)
                    {
                        userMessage = "Không thể xóa người dùng vì đã có vé được đặt. Vui lòng hủy vé trước.";
                    }
                    
                    return Json(new { success = false, message = userMessage });
                }
            }
            
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

        [HttpGet]
        public async Task<IActionResult> DiagnosePasswordIssues()
        {
            try
            {
                var usersWithInvalidPasswords = await _authService.GetUsersWithInvalidPasswordsAsync();
                
                var result = new
                {
                    success = true,
                    totalUsers = await _context.Users.CountAsync(),
                    usersWithInvalidPasswords = usersWithInvalidPasswords.Count,
                    invalidUsers = usersWithInvalidPasswords.Select(u => new
                    {
                        userId = u.UserId,
                        username = u.Username,
                        email = u.Email,
                        currentPasswordHash = u.Password,
                        passwordLength = u.Password?.Length ?? 0,
                        isValidFormat = !string.IsNullOrEmpty(u.Password) && u.Password.StartsWith("$2") && u.Password.Length >= 60
                    }).ToList()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error diagnosing password issues: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FixUserPassword(int userId, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    return Json(new { success = false, message = "New password cannot be empty" });
                }

                var result = await _authService.FixUserPasswordAsync(userId, newPassword);
                
                if (result)
                {
                    return Json(new { success = true, message = "Password fixed successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to fix password" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fixing password: " + ex.Message });
            }
        }
    }
}
