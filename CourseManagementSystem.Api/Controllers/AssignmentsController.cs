#nullable enable
using Microsoft.AspNetCore.Mvc;
using CourseManagementSystem.Core.DTOs;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Core.Interfaces;

namespace CourseManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentRepository _repo;
        private readonly ILogger<AssignmentsController> _logger;

        public AssignmentsController(IAssignmentRepository repo, ILogger<AssignmentsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _repo.GetAllAsync();
                var result = list.Select(a => ToResponseDto(a));
                return Ok(new { assignments = result, total = result.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assignments");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var a = await _repo.GetByIdAsync(id);
                if (a == null) return NotFound(new { message = $"Assignment {id} not found" });
                return Ok(ToResponseDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assignment {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-teacher-id/{teacherId}")]
        public async Task<IActionResult> GetByTeacher(int teacherId)
        {
            try
            {
                var list = await _repo.GetByTeacherIdAsync(teacherId);
                var result = list.Select(a => ToResponseDto(a));
                return Ok(new { assignments = result, total = result.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assignments for teacher {TeacherId}", teacherId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            try
            {
                var list = await _repo.GetByCourseIdAsync(courseId);
                var result = list.Select(a => ToResponseDto(a));
                return Ok(new { assignments = result, total = result.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assignments for course {CourseId}", courseId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssignmentCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var assignment = new Assignment
                {
                    Title       = dto.Title,
                    Description = dto.Description,
                    DueDate     = dto.DueDate,
                    TotalMarks  = dto.TotalMarks > 0 ? dto.TotalMarks : 100,
                    CourseId    = dto.CourseId,
                    TeacherId   = dto.TeacherId,
                    CreatedAt   = DateTime.UtcNow
                };

                var created = await _repo.AddAsync(assignment);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponseDto(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating assignment");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AssignmentUpdateDto dto)
        {
            try
            {
                if (id != dto.Id) return BadRequest(new { message = "ID mismatch" });
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var assignment = new Assignment
                {
                    Id          = dto.Id,
                    Title       = dto.Title,
                    Description = dto.Description,
                    DueDate     = dto.DueDate,
                    TotalMarks  = dto.TotalMarks > 0 ? dto.TotalMarks : 100,
                    CourseId    = dto.CourseId,
                    TeacherId   = dto.TeacherId
                };

                var updated = await _repo.UpdateAsync(assignment);
                if (updated == null) return NotFound(new { message = $"Assignment {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating assignment {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _repo.DeleteAsync(id);
                if (!result) return NotFound(new { message = $"Assignment {id} not found" });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assignment {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("submissions/{assignmentId}")]
        public async Task<IActionResult> GetSubmissions(int assignmentId)
        {
            try
            {
                var subs = await _repo.GetSubmissionsByAssignmentIdAsync(assignmentId);
                var result = subs.Select(s => ToSubmissionDto(s));
                return Ok(new { submissions = result, total = result.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting submissions for assignment {Id}", assignmentId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("my-submissions/{studentId}")]
        public async Task<IActionResult> GetMySubmissions(int studentId)
        {
            try
            {
                var subs = await _repo.GetSubmissionsByStudentIdAsync(studentId);
                var result = subs.Select(s => ToSubmissionDto(s));
                return Ok(new { submissions = result, total = result.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting submissions for student {Id}", studentId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmissionCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var existing = await _repo.GetSubmissionAsync(dto.AssignmentId, dto.StudentId);
                if (existing != null)
                    return BadRequest(new { message = "You have already submitted this assignment." });

                if (string.IsNullOrEmpty(dto.SubmissionUrl) && string.IsNullOrEmpty(dto.Content))
                    return BadRequest(new { message = "SubmissionUrl অথবা Content — যেকোনো একটা দিতে হবে।" });

                var submission = new AssignmentSubmission
                {
                    AssignmentId  = dto.AssignmentId,
                    StudentId     = dto.StudentId,
                    SubmissionUrl = dto.SubmissionUrl,
                    Content       = dto.Content,
                    Status        = "Submitted",
                    SubmittedAt   = DateTime.UtcNow
                };

                var created = await _repo.AddSubmissionAsync(submission);
                return CreatedAtAction(nameof(GetSubmissions), new { assignmentId = dto.AssignmentId }, ToSubmissionDto(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting assignment");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("grade/{submissionId}")]
        public async Task<IActionResult> Grade(int submissionId, [FromBody] GradeSubmissionDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var submission = await _repo.GetSubmissionByIdAsync(submissionId);
                if (submission == null)
                    return NotFound(new { message = $"Submission {submissionId} not found" });

                submission.MarksObtained = dto.Marks;
                submission.Feedback      = dto.Feedback;
                submission.Status        = "Graded";

                var updated = await _repo.UpdateSubmissionAsync(submission);
                if (updated == null) return NotFound(new { message = "Update failed" });

                return Ok(new { message = "Graded successfully!", submission = ToSubmissionDto(updated) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error grading submission {Id}", submissionId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private static AssignmentResponseDto ToResponseDto(Assignment a) => new()
        {
            Id          = a.Id,
            Title       = a.Title,
            Description = a.Description,
            DueDate     = a.DueDate,
            TotalMarks  = a.TotalMarks,
            CreatedAt   = a.CreatedAt,
            CourseId    = a.CourseId,
            CourseName  = a.Course?.Title,
            TeacherId   = a.TeacherId,
            TeacherName = a.Teacher?.Name,
            StudentName = a.Student?.Name,
        };

        private static SubmissionResponseDto ToSubmissionDto(AssignmentSubmission s) => new()
        {
            Id            = s.Id,
            AssignmentId  = s.AssignmentId,
            StudentId     = s.StudentId,
            StudentName   = s.Student?.Name,
            SubmissionUrl = s.SubmissionUrl,
            Content       = s.Content,
            Status        = s.Status,
            MarksObtained = s.MarksObtained,
            Feedback      = s.Feedback,
            SubmittedAt   = s.SubmittedAt,
        };
    }
}