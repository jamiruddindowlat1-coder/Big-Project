using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly string _jwtKey;

        public AuthController(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not found");
        }

        public class UserLoginDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class UserRegisterDto
        {
            public string Name { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = "student";
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Email))
                return BadRequest(new { message = "All fields are required!" });

            if (_db.Users.Any(u => u.Email == dto.Email))
                return BadRequest(new { message = "User already exists!" });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new { message = $"User {dto.Name} registered successfully!" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto credentials)
        {
            if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password))
                return BadRequest(new { message = "Username and password are required!" });

            var user = _db.Users.FirstOrDefault(u =>
                u.Email == credentials.Username ||
                u.Name == credentials.Username);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(credentials.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid credentials" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
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
                id = user.Id,
                username = user.Name,
                name = user.Name,
                email = user.Email,
                role = user.Role,
                salary = user.Salary,
                nid = user.Nid,
                address = user.Address,
                phone = user.Phone,
                designation = user.Expertise || user.Role,
                status = user.Status,
                joinDate = user.JoinDate,
                profilePicture = user.ProfilePicture
            });
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult Profile()
        {
            var email = this.User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new { 
                id = user.Id, 
                name = user.Name, 
                email = user.Email, 
                role = user.Role,
                salary = user.Salary,
                nid = user.Nid,
                address = user.Address,
                phone = user.Phone,
                designation = user.Expertise || user.Role,
                status = user.Status,
                joinDate = user.JoinDate,
                profilePicture = user.ProfilePicture
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var username = this.User.Identity?.Name ?? "Unknown";
            return Ok(new { message = $"User {username} logged out successfully!" });
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var userList = _db.Users.Select(u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email,
                role = u.Role
            }).ToList();

            return Ok(new { totalUsers = userList.Count, users = userList });
        }
    }
}