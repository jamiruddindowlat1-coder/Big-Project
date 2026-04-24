using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.DTOs;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Payments (Old)")]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(ApplicationDbContext context, ILogger<PaymentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class SimplePaymentDto
        {
            public int StudentId { get; set; }
            public int CourseId { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; } = "Completed";
            public string Category { get; set; } = "Student Fee";
            public DateTime Date { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Student)
                .Include(p => p.Course)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var result = payments.Select(p => new {
                id = p.Id,
                studentId = p.StudentId,
                studentName = p.Student?.Name,
                courseId = p.CourseId,
                courseName = p.Course?.Title,
                amount = p.Amount,
                date = p.CreatedAt,
                status = p.Status
            });

            return Ok(new { payments = result });
        }

        [HttpPost]
        public async Task<ActionResult> CreatePayment([FromBody] SimplePaymentDto dto)
        {
            var payment = new Payment {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Amount = dto.Amount,
                Status = dto.Status ?? "Completed",
                CreatedAt = DateTime.UtcNow
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Payment created", id = payment.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment payment)
        {
            if (id != payment.Id) return BadRequest();
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}