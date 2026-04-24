using Microsoft.AspNetCore.Mvc;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CourseManagementSystem.Api.Controllers
{
    public class CreateStudentDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Status { get; set; } = "active";
        public string? ProfilePicture { get; set; }
        public int? CourseId { get; set; }
        public string? Batch { get; set; }
    }

    public class UpdateStudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public string? ProfilePicture { get; set; }
        public int? CourseId { get; set; }
        public string? Batch { get; set; }
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
                .Where(u => u.Role == "Student" || u.Role == "student")
                .ToListAsync();
            return Ok(students);
        }

        // GET: api/students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetStudent(int id)
        {
            var student = await _context.Users.FindAsync(id);
            if (student == null || (student.Role != "Student" && student.Role != "student"))
                return NotFound();
            return Ok(student);
        }

        // POST: api/students
        [HttpPost]
        public async Task<ActionResult<User>> CreateStudent(CreateStudentDto dto)
        {
            // Ensure unique email
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { message = "Email is already in use." });
            }

            var student = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = "student",
                Phone = dto.Phone,
                Status = dto.Status ?? "active",
                ProfilePicture = dto.ProfilePicture,
                CreatedAt = DateTime.UtcNow
            };

            // Use BCrypt for consistent password hashing
            var password = string.IsNullOrWhiteSpace(dto.Password) ? "123456" : dto.Password;
            student.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            // Handle Enrollment if CourseId is provided
            if (dto.CourseId.HasValue && dto.CourseId.Value > 0)
            {
                var course = await _context.Courses.FindAsync(dto.CourseId.Value);
                if (course != null)
                {
                    var enrollment = new Enrollment
                    {
                        StudentId = student.Id,
                        CourseId = course.Id,
                        EnrollmentDate = DateTime.UtcNow,
                        TotalFee = course.Fee,
                        PaidAmount = 0,
                        InstallmentCount = course.AllowInstallment ? course.NumberOfInstallments : 1,
                        PaymentStatus = "Unpaid"
                    };
                    _context.Enrollments.Add(enrollment);
                    await _context.SaveChangesAsync();
                }
            }

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        // PUT: api/students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "ID mismatch." });

            var existing = await _context.Users.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Student not found." });

            if (!string.Equals(existing.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
                    return BadRequest(new { message = "Email is already in use." });
                existing.Email = dto.Email;
            }

            existing.Name = dto.Name;
            
            if (dto.Phone != null) existing.Phone = dto.Phone;
            if (dto.Status != null) existing.Status = dto.Status;
            if (dto.ProfilePicture != null) existing.ProfilePicture = dto.ProfilePicture;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Student updated successfully!" });
        }

        // GET: api/students/5/dashboard — Integrated dashboard for frontend
        [HttpGet("{id}/dashboard")]
        public async Task<IActionResult> GetStudentDashboard(int id)
        {
            var student = await _context.Users
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Category)
                .Include(u => u.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(u => u.Id == id && (u.Role.ToLower() == "student"));

            if (student == null)
                return NotFound(new { success = false, message = "Student not found" });

            var totalPaid = student.Enrollments?.Sum(e => e.PaidAmount) ?? 0;
            var totalPaymentsCount = await _context.Payments.CountAsync(p => p.StudentId == id);

            return Ok(new
            {
                success = true,
                student = new
                {
                    id = student.Id,
                    name = student.Name,
                    email = student.Email,
                    phone = student.Phone,
                    status = student.Status,
                    enrollments = student.Enrollments?.Select(e => new
                    {
                        id = e.Id,
                        courseId = e.CourseId,
                        courseTitle = e.Course?.Title,
                        instructorName = e.Course?.Instructor?.Name,
                        categoryName = e.Course?.Category?.Name,
                        enrollmentDate = e.EnrollmentDate,
                        batch = e.Course?.CourseCode,
                        paidAmount = e.PaidAmount,
                        totalFee = e.TotalFee,
                        paymentStatus = e.PaymentStatus
                    }),
                    totalPaymentsCount = totalPaymentsCount,
                    totalPaid = totalPaid
                }
            });
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