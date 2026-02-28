using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Core.Models;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseManagementSystem.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResult> RegisterAsync(string username, string email, string password)
        {
            try
            {
                // Check if user already exists
                if (await UserExistsAsync(email))
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
                }

                // Check username (using Name field)
                var existingUsername = await _context.Users
                    .FirstOrDefaultAsync(u => u.Name == username);

                if (existingUsername != null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Username already taken"
                    };
                }

                // Hash password
                var passwordHash = HashPassword(password);

                // Create new user
                var user = new User
                {
                    Name = username,  // ✅ Username থেকে Name করা হয়েছে
                    Email = email,
                    PasswordHash = passwordHash,
                    Role = "Student",  // ✅ Default role add করা হয়েছে
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user);

                return new AuthResult
                {
                    Success = true,
                    Message = "Registration successful",
                    Token = token,
                    UserId = user.Id,
                    Username = user.Name  // ✅ Username থেকে Name করা হয়েছে
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            try
            {
                // Find user by username (Name field) or email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Name == username || u.Email == username);

                if (user == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // Verify password
                var passwordHash = HashPassword(password);
                if (user.PasswordHash != passwordHash)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                return new AuthResult
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    UserId = user.Id,
                    Username = user.Name  // ✅ Username থেকে Name করা হয়েছে
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "YourVerySecureSecretKeyMinimum32CharactersLongForProduction!";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Name ?? user.Email),  // ✅ Username থেকে Name করা হয়েছে
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Simple password hashing - Use BCrypt or ASP.NET Identity in production!
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}