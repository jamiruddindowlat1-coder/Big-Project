using Microsoft.AspNetCore.Mvc;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Api.Controllers
{
    // ✅ Student তৈরির জন্য আলাদা DTO
    public class CreateStudentDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetStudents()
        {
            var students = await _context.Users
                .Where(u => u.Role == "Student")
                .ToListAsync();
            return Ok(students);
        }

        // GET: api/students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetStudent(int id)
        {
            var student = await _context.Users.FindAsync(id);
            if (student == null)
                return NotFound();
            return Ok(student);
        }

        // POST: api/students
        [HttpPost]
        public async Task<ActionResult<User>> CreateStudent(CreateStudentDto dto)
        {
            // ✅ Password hash করা হচ্ছে
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dto.Password ?? "defaultPass"));
            var passwordHash = Convert.ToBase64String(hashedBytes);

            var student = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash, // ✅ NULL আর হবে না
                Role = "Student",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(student);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        // PUT: api/students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, User student)
        {
            if (id != student.Id)
                return BadRequest();
            var existing = await _context.Users.FindAsync(id);
            if (existing == null)
                return NotFound();
            existing.Name = student.Name;
            existing.Email = student.Email;
            if (!string.IsNullOrEmpty(student.PasswordHash))
                existing.PasswordHash = student.PasswordHash;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Student updated successfully!" });
        }

        // DELETE: api/students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Users.FindAsync(id);
            if (student == null)
                return NotFound();
            _context.Users.Remove(student);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Student deleted successfully!" });
        }
    }
}