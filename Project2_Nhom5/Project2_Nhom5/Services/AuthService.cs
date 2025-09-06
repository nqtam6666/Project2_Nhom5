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
        Task<List<User>> GetUsersWithInvalidPasswordsAsync();
        Task<bool> FixUserPasswordAsync(int userId, string newPassword);
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
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    //Console.WriteLine("AuthService: Username or password is null or empty");
                    return null;
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    //Console.WriteLine($"AuthService: User '{username}' not found");
                    return null;
                }

                //Console.WriteLine($"AuthService: Found user '{username}', attempting password verification");
                
                if (VerifyPassword(password, user.Password))
                {
                    //Console.WriteLine($"AuthService: Password verification successful for user '{username}'");
                    return user;
                }
                else
                {
                    //Console.WriteLine($"AuthService: Password verification failed for user '{username}'");
                    return null;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"AuthService: Error in AuthenticateUserAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    //Console.WriteLine($"AuthService: Inner exception: {ex.InnerException.Message}");
                }
                return null;
            }
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                //Console.WriteLine($"AuthService: Starting registration for {user.Username}");
                
                // Kiểm tra username đã tồn tại chưa
                var existingUserByUsername = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username);

                if (existingUserByUsername != null)
                {
                    //Console.WriteLine($"AuthService: Username {user.Username} already exists");
                    return false; // Username đã tồn tại
                }

                // Kiểm tra email đã tồn tại chưa
                var existingUserByEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUserByEmail != null)
                {
                    //Console.WriteLine($"AuthService: Email {user.Email} already exists");
                    return false; // Email đã tồn tại
                }

                //Console.WriteLine($"AuthService: No conflicts found, proceeding with registration");

                // Hash password trước khi lưu
                user.Password = HashPassword(user.Password);
                //Console.WriteLine($"AuthService: Password hashed successfully");
                
                // Set default values theo constraint của database
                user.Status = "hoatdong"; // Theo constraint: 'hoatdong' hoặc 'khonghoatdong'
                if (string.IsNullOrEmpty(user.Role))
                {
                    user.Role = "NguoiDung"; // Theo constraint: 'NguoiDung', 'Admin', 'DaiLy'
                }

                //Console.WriteLine($"AuthService: Adding user to context");
                _context.Users.Add(user);
                
                //Console.WriteLine($"AuthService: Saving changes to database");
                await _context.SaveChangesAsync();
                
                //Console.WriteLine($"AuthService: User {user.Username} registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                //Console.WriteLine($"❌ AuthService: Lỗi khi đăng ký user: {ex.Message}");
                if (ex.InnerException != null)
                {
                    //Console.WriteLine($"❌ AuthService: Inner exception: {ex.InnerException.Message}");
                }
                
                // Log stack trace để debug
                //Console.WriteLine($"❌ AuthService: Stack trace: {ex.StackTrace}");
                
                return false;
            }
        }

        public string HashPassword(string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("Password cannot be null or empty", nameof(password));
                }

                // Generate a new salt and hash the password
                var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
                
                //Console.WriteLine($"AuthService: Password hashed successfully with salt length: {salt.Length}");
                return hashedPassword;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"AuthService: Error hashing password: {ex.Message}");
                throw;
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Check if hashedPassword is null or empty
                if (string.IsNullOrEmpty(hashedPassword))
                {
                    //Console.WriteLine("AuthService: Hashed password is null or empty");
                    return false;
                }

                // Check if hashedPassword looks like a valid BCrypt hash
                if (!hashedPassword.StartsWith("$2") || hashedPassword.Length < 60)
                {
                    //Console.WriteLine($"AuthService: Invalid BCrypt hash format: {hashedPassword}");
                    return false;
                }

                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (BCrypt.Net.SaltParseException ex)
            {
                //Console.WriteLine($"AuthService: BCrypt salt parsing error: {ex.Message}");
                //Console.WriteLine($"AuthService: Problematic hash: {hashedPassword}");
                return false;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"AuthService: Unexpected error in VerifyPassword: {ex.Message}");
                return false;
            }
        }

        public async Task<List<User>> GetUsersWithInvalidPasswordsAsync()
        {
            var usersWithInvalidPasswords = new List<User>();
            
            try
            {
                var allUsers = await _context.Users.ToListAsync();
                
                foreach (var user in allUsers)
                {
                    if (string.IsNullOrEmpty(user.Password) || 
                        !user.Password.StartsWith("$2") || 
                        user.Password.Length < 60)
                    {
                        usersWithInvalidPasswords.Add(user);
                        //Console.WriteLine($"AuthService: Found user with invalid password hash: {user.Username}");
                    }
                }
                
                //Console.WriteLine($"AuthService: Found {usersWithInvalidPasswords.Count} users with invalid password hashes");
                return usersWithInvalidPasswords;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"AuthService: Error getting users with invalid passwords: {ex.Message}");
                return usersWithInvalidPasswords;
            }
        }

        public async Task<bool> FixUserPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    //Console.WriteLine($"AuthService: User with ID {userId} not found");
                    return false;
                }

                var oldPassword = user.Password;
                user.Password = HashPassword(newPassword);
                
                await _context.SaveChangesAsync();
                
                //Console.WriteLine($"AuthService: Successfully fixed password for user {user.Username}");
                //Console.WriteLine($"AuthService: Old hash: {oldPassword}");
                //Console.WriteLine($"AuthService: New hash: {user.Password}");
                
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"AuthService: Error fixing password for user {userId}: {ex.Message}");
                return false;
            }
        }
    }
} 