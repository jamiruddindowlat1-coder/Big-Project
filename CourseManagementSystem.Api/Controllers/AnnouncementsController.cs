using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnnouncementsController> _logger;

        public AnnouncementsController(ApplicationDbContext context, ILogger<AnnouncementsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class AnnouncementDto
        {
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public string Type { get; set; } = "info";
            public string TargetAudience { get; set; } = "all";
            public bool IsActive { get; set; } = true;
            public string? BatchNo { get; set; }
            public string? RoomNo { get; set; }
            public string? ClassSchedule { get; set; }
            public string? Subject { get; set; }
            public DateTime? ClassDateTime { get; set; }
            // Optional relations (not stored as FK, stored inline if needed)
            public int? TeacherId { get; set; }
            public int? CategoryId { get; set; }
            public int? CourseId { get; set; }
        }

        private static object MapAnn(Announcement a) => new
        {
            id             = a.Id,
            title          = a.Title,
            content        = a.Content,
            type           = a.Type,
            targetAudience = a.TargetAudience,
            isActive       = a.IsActive,
            batchNo        = a.BatchNo,
            roomNo         = a.RoomNo,
            subject        = a.Subject,
            courseId       = a.CourseId,
            classDateTime  = a.ClassDateTime,
            createdAt      = a.CreatedAt
        };

        // GET /api/Announcements — All (returned as { announcements, total })
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _context.Announcements
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
                return Ok(new { announcements = list.Select(MapAnn), total = list.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll Announcements error");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/Announcements/all-admin — Admin: all announcements
        [HttpGet("all-admin")]
        public async Task<IActionResult> GetAllAdmin()
        {
            try
            {
                var list = await _context.Announcements
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
                return Ok(new { announcements = list.Select(MapAnn), total = list.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/Announcements/student/{studentId}
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetForStudent(int studentId)
        {
            try
            {
                // Get student's enrolled course IDs
                var enrolledCourseIds = await _context.Enrollments
                    .Where(e => e.StudentId == studentId)
                    .Select(e => e.CourseId)
                    .ToListAsync();

                var list = await _context.Announcements
                    .Where(a => a.IsActive && 
                           (a.TargetAudience == "all" || a.TargetAudience == "student") &&
                           (a.CourseId == null || enrolledCourseIds.Contains(a.CourseId.Value)))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                return Ok(new { announcements = list.Select(MapAnn) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/Announcements/teacher/{teacherId}
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetForTeacher(int teacherId)
        {
            try
            {
                // Get teacher's courses
                var teacherCourseIds = await _context.Courses
                    .Where(c => c.InstructorId == teacherId)
                    .Select(c => c.Id)
                    .ToListAsync();

                var list = await _context.Announcements
                    .Where(a => a.IsActive && 
                           (a.TargetAudience == "all" || a.TargetAudience == "teacher") &&
                           (a.CourseId == null || teacherCourseIds.Contains(a.CourseId.Value)))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                return Ok(new { announcements = list.Select(MapAnn) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/Announcements/by-audience/{audience}
        [HttpGet("by-audience/{audience}")]
        public async Task<IActionResult> GetByAudience(string audience)
        {
            try
            {
                var list = await _context.Announcements
                    .Where(a => a.IsActive && (a.TargetAudience == "all" || a.TargetAudience == audience.ToLower()))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
                return Ok(new { announcements = list.Select(MapAnn) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST /api/Announcements — Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AnnouncementDto dto)
        {
            try
            {
                var ann = new Announcement
                {
                    Title          = dto.Title,
                    Content        = dto.Content,
                    Type           = dto.Type,
                    TargetAudience = dto.TargetAudience,
                    IsActive       = dto.IsActive,
                    BatchNo        = dto.BatchNo,
                    RoomNo         = dto.RoomNo,
                    Subject        = dto.Subject,
                    CourseId       = dto.CourseId,
                    ClassDateTime  = dto.ClassDateTime?.ToUniversalTime(),
                    CreatedAt      = DateTime.UtcNow
                };

                _context.Announcements.Add(ann);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Announcement created!", id = ann.Id, announcement = MapAnn(ann) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Announcement error");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT /api/Announcements/{id} — Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AnnouncementDto dto)
        {
            try
            {
                var ann = await _context.Announcements.FindAsync(id);
                if (ann == null) return NotFound(new { message = "Announcement not found" });

                ann.Title          = dto.Title;
                ann.Content        = dto.Content;
                ann.Type           = dto.Type;
                ann.TargetAudience = dto.TargetAudience;
                ann.IsActive       = dto.IsActive;
                ann.BatchNo        = dto.BatchNo;
                ann.RoomNo         = dto.RoomNo;
                ann.Subject        = dto.Subject;
                ann.CourseId       = dto.CourseId;
                ann.ClassDateTime  = dto.ClassDateTime?.ToUniversalTime();

                await _context.SaveChangesAsync();
                return Ok(new { message = "Updated!", announcement = MapAnn(ann) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE /api/Announcements/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ann = await _context.Announcements.FindAsync(id);
                if (ann == null) return NotFound(new { message = "Not found" });

                _context.Announcements.Remove(ann);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PATCH /api/Announcements/{id}/toggle
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var ann = await _context.Announcements.FindAsync(id);
                if (ann == null) return NotFound(new { message = "Not found" });

                ann.IsActive = !ann.IsActive;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Toggled", isActive = ann.IsActive });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
