using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Core.Models.Entities;

namespace CourseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/Enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class EnrollmentDto
        {
            public int StudentId { get; set; }
            public int CourseId { get; set; }
            public DateTime EnrollmentDate { get; set; }
            public decimal TotalFee { get; set; }
            public decimal PaidAmount { get; set; }
            public int InstallmentCount { get; set; }
            public string PaymentStatus { get; set; } = "Unpaid";
        }

        // ✅ GET ALL — GET /api/Enrollments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ThenInclude(c => c.Category)
                .Select(e => new
                {
                    id = e.Id,
                    studentId = e.StudentId,
                    studentName = e.Student != null ? e.Student.Name : "Unknown",
                    studentEmail = e.Student != null ? e.Student.Email : "Unknown",
                    courseId = e.CourseId,
                    courseTitle = e.Course != null ? e.Course.Title : "Unknown",
                    categoryName = e.Course != null && e.Course.Category != null ? e.Course.Category.Name : "Unknown",
                    enrollmentDate = e.EnrollmentDate,
                    totalFee = e.TotalFee,
                    paidAmount = e.PaidAmount,
                    installmentCount = e.InstallmentCount,
                    paymentStatus = e.PaymentStatus
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // ✅ CREATE — POST /api/Enrollments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnrollmentDto dto)
        {
            var enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                EnrollmentDate = dto.EnrollmentDate,
                TotalFee = dto.TotalFee,
                PaidAmount = dto.PaidAmount,
                InstallmentCount = dto.InstallmentCount,
                PaymentStatus = dto.PaymentStatus
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrollment created successfully!", id = enrollment.Id });
        }

        // ✅ UPDATE — PUT /api/Enrollments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EnrollmentDto dto)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found!" });

            enrollment.StudentId = dto.StudentId;
            enrollment.CourseId = dto.CourseId;
            enrollment.EnrollmentDate = dto.EnrollmentDate;
            enrollment.TotalFee = dto.TotalFee;
            enrollment.PaidAmount = dto.PaidAmount;
            enrollment.InstallmentCount = dto.InstallmentCount;
            enrollment.PaymentStatus = dto.PaymentStatus;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrollment updated successfully!" });
        }

        // ✅ DELETE — DELETE /api/Enrollments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found!" });

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrollment deleted successfully!" });
        }
    }
}