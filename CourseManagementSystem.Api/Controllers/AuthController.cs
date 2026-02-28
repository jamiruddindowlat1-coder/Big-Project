using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace CourseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string _jwtKey = "supersecret_jwt_key_1234567890123456";
        private static List<AppUser> _users = new List<AppUser>();

        // ✅ Renamed to AppUser to avoid conflict with ClaimsPrincipal.User
        public class AppUser
        {
            public string Username { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        public class UserLoginDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class UserRegisterDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        // ✅ REGISTER ENDPOINT
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto user)
        {
            // Validation
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Email))
                return BadRequest(new { message = "All fields are required!" });

            // Check if user already exists
            if (_users.Any(u => u.Username == user.Username))
                return BadRequest(new { message = "User already exists!" });

            // Hash password using BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Save user to in-memory list
            _users.Add(new AppUser
            {
                Username = user.Username,
                PasswordHash = hashedPassword,
                Email = user.Email
            });

            return Ok(new { message = $"User {user.Username} registered successfully!" });
        }

        // ✅ LOGIN ENDPOINT
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto credentials)
        {
            // Validation
            if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password))
                return BadRequest(new { message = "Username and password are required!" });

            // Find user by username
            var user = _users.FirstOrDefault(u => u.Username == credentials.Username);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // Verify password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(credentials.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid credentials" });

            // Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                message = "Login successful",
                token = tokenString,
                username = user.Username,
                email = user.Email
            });
        }

        // ✅ PROFILE ENDPOINT (Protected)
        [HttpGet("profile")]
        [Authorize]
        public IActionResult Profile()
        {
            // ✅ Fixed: Use this.User instead of just User
            var username = this.User.Identity?.Name ?? "Unknown";
            var user = _users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                username = user.Username,
                email = user.Email
            });
        }

        // ✅ LOGOUT ENDPOINT (Protected)
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // ✅ Fixed: Use this.User instead of just User
            var username = this.User.Identity?.Name ?? "Unknown";
            return Ok(new { message = $"User {username} logged out successfully!" });
        }

        // ✅ GET ALL USERS (For Testing)
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var userList = _users.Select(u => new
            {
                username = u.Username,
                email = u.Email
            });

            return Ok(new
            {
                totalUsers = _users.Count,
                users = userList
            });
        }
    }
}