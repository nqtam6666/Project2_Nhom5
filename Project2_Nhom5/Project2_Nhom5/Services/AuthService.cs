using Project2_Nhom5.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Project2_Nhom5.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateUserAsync(string username, string password);
        Task<bool> RegisterUserAsync(User user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }

    public class AuthService : IAuthService
    {
        private readonly Project2_Nhom5Context _context;

        public AuthService(Project2_Nhom5Context context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && VerifyPassword(password, user.Password))
            {
                return user;
            }

            return null;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                Console.WriteLine($"AuthService: Starting registration for {user.Username}");
                
                // Kiểm tra username đã tồn tại chưa
                var existingUserByUsername = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username);

                if (existingUserByUsername != null)
                {
                    Console.WriteLine($"AuthService: Username {user.Username} already exists");
                    return false; // Username đã tồn tại
                }

                // Kiểm tra email đã tồn tại chưa
                var existingUserByEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUserByEmail != null)
                {
                    Console.WriteLine($"AuthService: Email {user.Email} already exists");
                    return false; // Email đã tồn tại
                }

                Console.WriteLine($"AuthService: No conflicts found, proceeding with registration");

                // Hash password trước khi lưu
                user.Password = HashPassword(user.Password);
                Console.WriteLine($"AuthService: Password hashed successfully");
                
                // Set default values theo constraint của database
                user.Status = "hoatdong"; // Theo constraint: 'hoatdong' hoặc 'khonghoatdong'
                if (string.IsNullOrEmpty(user.Role))
                {
                    user.Role = "NguoiDung"; // Theo constraint: 'NguoiDung', 'Admin', 'DaiLy'
                }

                Console.WriteLine($"AuthService: Adding user to context");
                _context.Users.Add(user);
                
                Console.WriteLine($"AuthService: Saving changes to database");
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"AuthService: User {user.Username} registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"❌ AuthService: Lỗi khi đăng ký user: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"❌ AuthService: Inner exception: {ex.InnerException.Message}");
                }
                
                // Log stack trace để debug
                Console.WriteLine($"❌ AuthService: Stack trace: {ex.StackTrace}");
                
                return false;
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
} 