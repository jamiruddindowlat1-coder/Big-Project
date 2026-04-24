using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CourseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class TeacherCreateDto
        {
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public decimal Salary { get; set; }
        }

        public class TeacherUpdateDto
        {
            public string Name { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public decimal Salary { get; set; }
        }

        // ✅ CREATE — POST /api/teacher/create
        [HttpPost("create")]
        public IActionResult Create([FromBody] TeacherCreateDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || 
                string.IsNullOrEmpty(request.Name) || 
                string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "All fields are required!" });

            if (_context.Users.Any(t => t.Email == request.Email))
                return BadRequest(new { message = "Teacher Email already exists!" });

            var teacher = new User
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "teacher",
                CreatedAt = DateTime.UtcNow,
                Status = "active",
                Salary = request.Salary
            };

            _context.Users.Add(teacher);
            _context.SaveChanges();

            return Ok(new { message = $"Teacher {request.Name} created successfully!" });
        }

        // ✅ GET ALL — GET /api/teacher/all
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var users = _context.Users.Where(u => u.Role.ToLower() == "teacher").ToList();

            var list = users.Select(t => new
            {
                id = t.Id, // integer
                teacherId = t.Email, // Map Email to teacherId for frontend compatibility
                name = t.Name,
                email = t.Email,
                salary = t.Salary
            });

            return Ok(new
            {
                totalTeachers = users.Count,
                teachers = list
            });
        }

        // ✅ UPDATE — PUT /api/teacher/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] TeacherUpdateDto request)
        {
            var teacher = _context.Users.FirstOrDefault(t => t.Id == id && t.Role.ToLower() == "teacher");

            if (teacher == null)
                return NotFound(new { message = "Teacher not found!" });

            if (!string.IsNullOrEmpty(request.Name))
                teacher.Name = request.Name;
                
            if (!string.IsNullOrEmpty(request.Email))
            {
                if (_context.Users.Any(t => t.Email == request.Email && t.Id != id))
                    return BadRequest(new { message = "Email already in use!" });
                teacher.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.Password))
                teacher.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            teacher.Salary = request.Salary;

            _context.SaveChanges();

            return Ok(new
            {
                message = $"Teacher {teacher.Name} updated successfully!",
                teacher = new
                {
                    id = teacher.Id,
                    teacherId = teacher.Email,
                    name = teacher.Name,
                    email = teacher.Email,
                    salary = teacher.Salary
                }
            });
        }

        // ✅ DASHBOARD — GET /api/teacher/{id}/dashboard
        [HttpGet("{id}/dashboard")]
        public async Task<IActionResult> GetTeacherDashboard(int id)
        {
            var teacher = await _context.Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(t => t.Id == id && t.Role.ToLower() == "teacher");

            if (teacher == null)
                return NotFound(new { message = "Teacher not found!" });

            // Fetch students across all their courses
            var courseIds = teacher.Courses.Select(c => c.Id).ToList();
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => courseIds.Contains(e.CourseId))
                .ToListAsync();

            var uniqueStudents = enrollments.Select(e => new 
            { 
                id = e.StudentId, 
                name = e.Student?.Name, 
                email = e.Student?.Email,
                course = e.Course?.Title,
                batch = e.Course?.CourseCode
            }).ToList();

            // Fetch schedules for these courses
            var schedules = await _context.ClassSchedules
                .Include(s => s.Course)
                .Where(s => courseIds.Contains(s.CourseId))
                .OrderBy(s => s.ClassDateTime)
                .ToListAsync();

            var classScheduleList = schedules.Select(s => new
            {
                day = s.ClassDateTime.DayOfWeek.ToString(),
                batch = s.Batch,
                subject = s.Course?.Title,
                time = s.ClassDateTime.ToString("hh:mm tt")
            }).ToList();

            return Ok(new
            {
                teacher = new
                {
                    id = teacher.Id,
                    name = teacher.Name,
                    email = teacher.Email,
                    expertise = teacher.Expertise,
                    courses = teacher.Courses.Select(c => new { id = c.Id, title = c.Title }),
                    students = uniqueStudents,
                    classSchedule = classScheduleList
                }
            });
        }

        // ✅ DELETE — DELETE /api/teacher/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var teacher = _context.Users.FirstOrDefault(t => t.Id == id && t.Role.ToLower() == "teacher");

            if (teacher == null)
                return NotFound(new { message = "Teacher not found!" });

            _context.Users.Remove(teacher);
            _context.SaveChanges();

            return Ok(new { message = $"Teacher {teacher.Name} deleted successfully!" });
        }
    }
}