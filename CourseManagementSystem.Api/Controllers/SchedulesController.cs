using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SchedulesController> _logger;

        public SchedulesController(ApplicationDbContext context, ILogger<SchedulesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class ScheduleCreateDto
        {
            public int CourseId { get; set; }
            public string Batch { get; set; } = string.Empty;
            public string RoomNo { get; set; } = string.Empty;
            public DateTime ClassDateTime { get; set; }
            public string? Note { get; set; }
        }

        // GET /api/Schedules — Admin: all schedules
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var schedules = await _context.ClassSchedules
                    .Include(s => s.Course)
                    .OrderByDescending(s => s.ClassDateTime)
                    .Select(s => new
                    {
                        id            = s.Id,
                        courseId      = s.CourseId,
                        courseName    = s.Course.Title,
                        batch         = s.Batch,
                        roomNo        = s.RoomNo,
                        classDateTime = s.ClassDateTime,
                        note          = s.Note,
                        createdAt     = s.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new { schedules, total = schedules.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedules");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/Schedules/by-course/{courseId}
        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            try
            {
                var schedules = await _context.ClassSchedules
                    .Include(s => s.Course)
                    .Where(s => s.CourseId == courseId)
                    .OrderBy(s => s.ClassDateTime)
                    .Select(s => new
                    {
                        id            = s.Id,
                        courseId      = s.CourseId,
                        courseName    = s.Course.Title,
                        batch         = s.Batch,
                        roomNo        = s.RoomNo,
                        classDateTime = s.ClassDateTime,
                        note          = s.Note
                    })
                    .ToListAsync();

                return Ok(new { schedules, total = schedules.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/Schedules/by-student/{studentId} — Student's enrolled course schedules
        [HttpGet("by-student/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            try
            {
                // Get all courses this student is enrolled in
                var enrolledCourseIds = await _context.Enrollments
                    .Where(e => e.StudentId == studentId)
                    .Select(e => e.CourseId)
                    .ToListAsync();

                var schedules = await _context.ClassSchedules
                    .Include(s => s.Course)
                    .Where(s => enrolledCourseIds.Contains(s.CourseId))
                    .OrderBy(s => s.ClassDateTime)
                    .Select(s => new
                    {
                        id            = s.Id,
                        courseId      = s.CourseId,
                        courseName    = s.Course.Title,
                        batch         = s.Batch,
                        roomNo        = s.RoomNo,
                        classDateTime = s.ClassDateTime,
                        note          = s.Note,
                        day           = s.ClassDateTime.DayOfWeek.ToString(),
                        startTime     = s.ClassDateTime.ToString("hh:mm tt"),
                        endTime       = s.ClassDateTime.AddHours(1.5).ToString("hh:mm tt"),
                        room          = s.RoomNo
                    })
                    .ToListAsync();

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedules for student {Id}", studentId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/Schedules/by-teacher/{teacherId}
        [HttpGet("by-teacher/{teacherId}")]
        public async Task<IActionResult> GetByTeacher(int teacherId)
        {
            try
            {
                // Get all courses this teacher teaches
                var teacherCourseIds = await _context.Courses
                    .Where(c => c.InstructorId == teacherId)
                    .Select(c => c.Id)
                    .ToListAsync();

                var schedules = await _context.ClassSchedules
                    .Include(s => s.Course)
                    .Where(s => teacherCourseIds.Contains(s.CourseId))
                    .OrderBy(s => s.ClassDateTime)
                    .Select(s => new
                    {
                        id            = s.Id,
                        courseId      = s.CourseId,
                        courseName    = s.Course.Title,
                        batch         = s.Batch,
                        roomNo        = s.RoomNo,
                        classDateTime = s.ClassDateTime,
                        note          = s.Note
                    })
                    .ToListAsync();

                return Ok(new { schedules, total = schedules.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST /api/Schedules — Admin creates schedule
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateDto dto)
        {
            try
            {
                var course = await _context.Courses.FindAsync(dto.CourseId);
                if (course == null)
                    return NotFound(new { message = "Course not found" });

                var schedule = new ClassSchedule
                {
                    CourseId      = dto.CourseId,
                    Batch         = dto.Batch,
                    RoomNo        = dto.RoomNo,
                    ClassDateTime = dto.ClassDateTime.ToUniversalTime(),
                    Note          = dto.Note,
                    CreatedAt     = DateTime.UtcNow
                };

                _context.ClassSchedules.Add(schedule);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Schedule created successfully!", id = schedule.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE /api/Schedules/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var schedule = await _context.ClassSchedules.FindAsync(id);
                if (schedule == null)
                    return NotFound(new { message = "Schedule not found" });

                _context.ClassSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
