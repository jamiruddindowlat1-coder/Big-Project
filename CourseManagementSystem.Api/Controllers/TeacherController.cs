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
    public class TeacherController : ControllerBase
    {
        private readonly string _jwtKey = "supersecret_jwt_key_1234567890123456";

        // ✅ Default Demo Teacher
        private static List<AppTeacher> _teachers = new List<AppTeacher>
        {
            new AppTeacher
            {
                TeacherId = "T001",
                Name = "Demo Teacher",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("teacher123")
            }
        };

        public class AppTeacher
        {
            public string TeacherId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
        }

        public class TeacherLoginDto
        {
            public string TeacherId { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class TeacherCreateDto
        {
            public string TeacherId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class TeacherUpdateDto
        {
            public string Name { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // ✅ LOGIN — POST /api/teacher/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] TeacherLoginDto credentials)
        {
            if (string.IsNullOrEmpty(credentials.TeacherId) || string.IsNullOrEmpty(credentials.Password))
                return BadRequest(new { message = "Teacher ID and Password are required!" });

            var teacher = _teachers.FirstOrDefault(t => t.TeacherId == credentials.TeacherId);

            if (teacher == null)
                return Unauthorized(new { message = "Invalid Teacher ID or Password" });

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(credentials.Password, teacher.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid Teacher ID or Password" });

            // ✅ JWT Token Generate
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, teacher.TeacherId),
                    new Claim("name", teacher.Name),
                    new Claim(ClaimTypes.Role, "Teacher")
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
                teacher = new
                {
                    id = teacher.TeacherId,
                    name = teacher.Name
                }
            });
        }

        // ✅ CREATE — POST /api/teacher/create
        [HttpPost("create")]
        public IActionResult Create([FromBody] TeacherCreateDto request)
        {
            if (string.IsNullOrEmpty(request.TeacherId) || 
                string.IsNullOrEmpty(request.Name) || 
                string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "All fields are required!" });

            if (_teachers.Any(t => t.TeacherId == request.TeacherId))
                return BadRequest(new { message = "Teacher ID already exists!" });

            _teachers.Add(new AppTeacher
            {
                TeacherId = request.TeacherId,
                Name = request.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            });

            return Ok(new { message = $"Teacher {request.Name} created successfully!" });
        }

        // ✅ GET ALL — GET /api/teacher/all
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var list = _teachers.Select(t => new
            {
                teacherId = t.TeacherId,
                name = t.Name
            });

            return Ok(new
            {
                totalTeachers = _teachers.Count,
                teachers = list
            });
        }

        // ✅ UPDATE — PUT /api/teacher/update/{teacherId}
        [HttpPut("update/{teacherId}")]
        public IActionResult Update(string teacherId, [FromBody] TeacherUpdateDto request)
        {
            if (string.IsNullOrEmpty(request.Name) && string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Name or Password is required to update!" });

            var teacher = _teachers.FirstOrDefault(t => t.TeacherId == teacherId);

            if (teacher == null)
                return NotFound(new { message = "Teacher not found!" });

            if (!string.IsNullOrEmpty(request.Name))
                teacher.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Password))
                teacher.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            return Ok(new
            {
                message = $"Teacher {teacher.Name} updated successfully!",
                teacher = new
                {
                    id = teacher.TeacherId,
                    name = teacher.Name
                }
            });
        }

        // ✅ DELETE — DELETE /api/teacher/delete/{teacherId}
        [HttpDelete("delete/{teacherId}")]
        public IActionResult Delete(string teacherId)
        {
            var teacher = _teachers.FirstOrDefault(t => t.TeacherId == teacherId);

            if (teacher == null)
                return NotFound(new { message = "Teacher not found!" });

            _teachers.Remove(teacher);

            return Ok(new { message = $"Teacher {teacher.Name} deleted successfully!" });
        }
    }
}