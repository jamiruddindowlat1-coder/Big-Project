#nullable enable
using Microsoft.AspNetCore.Mvc;
using CourseManagementSystem.Core.DTOs;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Core.Interfaces;

namespace CourseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseRepository courseRepository, ILogger<CoursesController> logger)
        {
            _courseRepository = courseRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetCourses()
        {
            var courses = await _courseRepository.GetAllAsync();
            var result = courses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                CourseCode = c.CourseCode,
                Credits = c.Credits,
                Duration = c.Duration,
                CategoryId = c.CategoryId,
                CategoryName = c.Category?.Name ?? string.Empty,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor?.Name ?? string.Empty,
                Fee = c.Fee,
                AllowInstallment = c.AllowInstallment,
                NumberOfInstallments = c.NumberOfInstallments
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponseDto>> GetCourse(int id)
        {
            var c = await _courseRepository.GetByIdAsync(id);
            if (c == null) return NotFound();
            var result = new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                CourseCode = c.CourseCode,
                Credits = c.Credits,
                Duration = c.Duration,
                CategoryId = c.CategoryId,
                CategoryName = c.Category?.Name ?? string.Empty,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor?.Name ?? string.Empty,
                Fee = c.Fee,
                AllowInstallment = c.AllowInstallment,
                NumberOfInstallments = c.NumberOfInstallments
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CourseResponseDto>> CreateCourse([FromBody] CourseCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                CourseCode = dto.CourseCode,
                Credits = dto.Credits,       // ✅ ?? 0 সরানো হয়েছে
                Duration = dto.Duration,     // ✅ ?? 0 সরানো হয়েছে
                CategoryId = dto.CategoryId,
                InstructorId = dto.InstructorId,
                Fee = dto.Fee,
                AllowInstallment = dto.AllowInstallment,
                NumberOfInstallments = dto.NumberOfInstallments
            };

            var created = await _courseRepository.AddAsync(course);

            var result = new CourseResponseDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                CourseCode = created.CourseCode,
                Credits = created.Credits,
                Duration = created.Duration,
                CategoryId = created.CategoryId,
                CategoryName = created.Category?.Name ?? string.Empty,
                InstructorId = created.InstructorId,
                InstructorName = created.Instructor?.Name ?? string.Empty,
                Fee = created.Fee,
                AllowInstallment = created.AllowInstallment,
                NumberOfInstallments = created.NumberOfInstallments
            };

            return CreatedAtAction(nameof(GetCourse), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpdateDto dto)
        {
            if (!ModelState.IsValid || id != dto.Id) return BadRequest();

            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return NotFound();

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.CourseCode = dto.CourseCode;
            course.Credits = dto.Credits;       // ✅ ?? 0 সরানো হয়েছে
            course.Duration = dto.Duration;     // ✅ ?? 0 সরানো হয়েছে
            course.CategoryId = dto.CategoryId;
            course.InstructorId = dto.InstructorId;
            course.Fee = dto.Fee;
            course.AllowInstallment = dto.AllowInstallment;
            course.NumberOfInstallments = dto.NumberOfInstallments;

            await _courseRepository.UpdateAsync(course);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var success = await _courseRepository.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}