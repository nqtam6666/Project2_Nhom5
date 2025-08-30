using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project2_Nhom5.Models;
using Project2_Nhom5.Areas.Guest.Models;
using Project2_Nhom5.Services;

namespace Project2_Nhom5.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly Project2_Nhom5Context _context;
        private readonly IAuthService _authService;

        public HomeController(Project2_Nhom5Context context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Request.Cookies["userId"];
            var username = Request.Cookies["username"];
            var role = Request.Cookies["role"] ?? string.Empty;
            
            ViewData["IsLoggedIn"] = !string.IsNullOrEmpty(userId);
            ViewData["Username"] = username;
            ViewData["IsAdmin"] = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

            var vm = new GuestHomeViewModel
            {
                NowShowing = await _context.Movies.Where(m => m.Status == "dangchieu").OrderByDescending(m => m.MovieId).Take(6).ToListAsync(),
                ComingSoon = await _context.Movies.Where(m => m.Status == "sapchieu").OrderByDescending(m => m.MovieId).Take(6).ToListAsync(),
                Stopped = await _context.Movies.Where(m => m.Status == "ngungchieu").OrderByDescending(m => m.MovieId).Take(6).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            try
            {
                var user = await _authService.AuthenticateUserAsync(model.Username, model.Password);
                
                if (user != null)
                {
                    // Kiểm tra trạng thái user
                    if (user.Status != "hoatdong")
                    {
                        return Json(new { success = false, message = "Tài khoản đã bị khóa hoặc không hoạt động" });
                    }

                    // Lưu thông tin user vào session/cookie
                    Response.Cookies.Append("userId", user.UserId.ToString(), new CookieOptions
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        MaxAge = TimeSpan.FromDays(30)
                    });
                    
                    Response.Cookies.Append("username", user.Username, new CookieOptions
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        MaxAge = TimeSpan.FromDays(30)
                    });
                    
                    Response.Cookies.Append("role", user.Role ?? "NguoiDung", new CookieOptions
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        MaxAge = TimeSpan.FromDays(30)
                    });

                    return Json(new { 
                        success = true, 
                        message = "Đăng nhập thành công", 
                        user = new { 
                            id = user.UserId, 
                            username = user.Username, 
                            role = user.Role,
                            email = user.Email,
                            status = user.Status
                        } 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                // Log dữ liệu đầu vào
                Console.WriteLine($"Register attempt - Username: {model.Username}, Email: {model.Email}");
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var errorMessage = string.Join(", ", errors);
                    Console.WriteLine($"ModelState errors: {errorMessage}");
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ: " + errorMessage });
                }

                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password, // Sẽ được hash trong service
                    Email = model.Email,
                    Phone = model.Phone,
                    Role = "NguoiDung", // Theo constraint database
                    Status = "hoatdong" // Theo constraint database
                };

                Console.WriteLine($"Creating user object: {user.Username}, {user.Email}");

                var result = await _authService.RegisterUserAsync(user);
                
                if (result)
                {
                    Console.WriteLine($"✅ User {user.Username} registered successfully");
                    return Json(new { success = true, message = "Đăng ký thành công" });
                }
                else
                {
                    Console.WriteLine($"❌ User {user.Username} registration failed");
                    
                    // Kiểm tra cụ thể lỗi gì
                    var existingUserByUsername = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == model.Username);
                    
                    var existingUserByEmail = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == model.Email);

                    if (existingUserByUsername != null)
                    {
                        Console.WriteLine($"Username {model.Username} already exists");
                        return Json(new { success = false, message = "Tên đăng nhập đã tồn tại" });
                    }
                    else if (existingUserByEmail != null)
                    {
                        Console.WriteLine($"Email {model.Email} already exists");
                        return Json(new { success = false, message = "Email đã được sử dụng" });
                    }
                    else
                    {
                        Console.WriteLine("Unknown registration error");
                        return Json(new { success = false, message = "Có lỗi xảy ra khi đăng ký" });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in Register: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"❌ Inner exception: {ex.InnerException.Message}");
                }
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("userId");
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("role");
            return Json(new { success = true, message = "Đăng xuất thành công" });
        }

        [HttpPost]
        public IActionResult LoginAsAdmin()
        {
            Response.Cookies.Append("role", "Admin", new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                MaxAge = TimeSpan.FromHours(8)
            });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TestRegister()
        {
            try
            {
                var testUser = new User
                {
                    Username = "testuser_" + DateTime.Now.Ticks,
                    Password = "test123",
                    Email = "test_" + DateTime.Now.Ticks + "@test.com",
                    Phone = "0123456789",
                    Role = "customer",
                    Status = "active"
                };

                var result = await _authService.RegisterUserAsync(testUser);
                
                if (result)
                {
                    return Json(new { success = true, message = "Test đăng ký thành công", username = testUser.Username });
                }
                else
                {
                    return Json(new { success = false, message = "Test đăng ký thất bại" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi test: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                var userCount = await _context.Users.CountAsync();
                var movieCount = await _context.Movies.CountAsync();
                
                // Kiểm tra cấu trúc bảng Users
                var sampleUser = await _context.Users.FirstOrDefaultAsync();
                var userProperties = sampleUser != null ? 
                    new { 
                        UserId = sampleUser.UserId,
                        Username = sampleUser.Username,
                        Email = sampleUser.Email,
                        Role = sampleUser.Role,
                        Status = sampleUser.Status
                    } : null;

                return Json(new { 
                    success = true, 
                    userCount = userCount,
                    movieCount = movieCount,
                    sampleUser = userProperties,
                    connectionString = _context.Database.GetConnectionString()
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Lỗi kiểm tra database: " + ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestSpecificRegister()
        {
            try
            {
                var testUser = new User
                {
                    Username = "quangtam1",
                    Password = "quangtam1",
                    Email = "1@gmail.com",
                    Phone = "111111111",
                    Role = "NguoiDung", // Theo constraint database
                    Status = "hoatdong" // Theo constraint database
                };

                var result = await _authService.RegisterUserAsync(testUser);
                
                if (result)
                {
                    return Json(new { success = true, message = "Test đăng ký thành công với dữ liệu cụ thể", username = testUser.Username });
                }
                else
                {
                    return Json(new { success = false, message = "Test đăng ký thất bại với dữ liệu cụ thể" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi test: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckConstraints()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                var userData = users.Select(u => new
                {
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    Status = u.Status,
                    ValidRole = new[] { "Admin", "NguoiDung", "DaiLy" }.Contains(u.Role),
                    ValidStatus = new[] { "hoatdong", "khonghoatdong" }.Contains(u.Status)
                }).ToList();

                return Json(new
                {
                    success = true,
                    totalUsers = users.Count,
                    users = userData,
                    constraints = new
                    {
                        validRoles = new[] { "Admin", "NguoiDung", "DaiLy" },
                        validStatuses = new[] { "hoatdong", "khonghoatdong" }
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi kiểm tra constraints: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = Request.Cookies["userId"];
                var username = Request.Cookies["username"];
                var role = Request.Cookies["role"];

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(username))
                {
                    // Lấy thông tin user từ database để kiểm tra status
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
                    
                    if (user != null)
                    {
                        return Json(new
                        {
                            success = true,
                            isLoggedIn = true,
                            user = new
                            {
                                id = userId,
                                username = username,
                                role = role ?? "NguoiDung",
                                status = user.Status
                            }
                        });
                    }
                    else
                    {
                        // User không tồn tại trong database, xóa cookies
                        Response.Cookies.Delete("userId");
                        Response.Cookies.Delete("username");
                        Response.Cookies.Delete("role");
                        
                        return Json(new
                        {
                            success = true,
                            isLoggedIn = false
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        isLoggedIn = false
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi kiểm tra user hiện tại: " + ex.Message });
            }
        }
    }
} 