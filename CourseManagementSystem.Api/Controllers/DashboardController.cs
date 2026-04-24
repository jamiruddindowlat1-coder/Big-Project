using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var totalStudents    = await _context.Users.CountAsync(u => u.Role == "Student");
                var totalTeachers    = await _context.Users.CountAsync(u => u.Role == "Teacher");
                var totalUsers       = await _context.Users.CountAsync();
                var activeCourses    = await _context.Courses.CountAsync();
                var totalCategories  = await _context.Categories.CountAsync();
                var totalEnrollments = await _context.Enrollments.CountAsync();
                var totalPayments    = await _context.Payments.CountAsync();
                var totalRevenue     = await _context.Accounts
                                        .Where(a => a.TransactionType == "CREDIT")
                                        .SumAsync(a => (decimal?)a.Amount) ?? 0;
                var totalExpenses    = await _context.Accounts
                                        .Where(a => a.TransactionType == "DEBIT")
                                        .SumAsync(a => (decimal?)a.Amount) ?? 0;

                return Ok(new
                {
                    totalStudents,
                    totalTeachers,
                    totalUsers,
                    activeCourses,
                    totalCategories,
                    totalEnrollments,
                    totalPayments,
                    totalRevenue,
                    totalExpenses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}