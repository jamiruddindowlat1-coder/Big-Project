using Microsoft.AspNetCore.Mvc;
using CourseManagementSystem.Core.DTOs;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Core.Interfaces;

namespace CourseManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            try
            {
                var courses = await _courseRepository.GetAllAsync();

                // ? Entity ? DTO (CategoryName ??? ????)
                var result = courses.Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category?.Name,        // ? ???? Frontend-? ??????
                    InstructorId = c.InstructorId,
                    InstructorName = c.Instructor?.Name  // Instructor ???
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponseDto>> GetCourse(int id)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null)
                    return NotFound(new { message = $"Course with id {id} not found" });

                // ? Entity ? DTO
                var result = new CourseResponseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    CategoryId = course.CategoryId,
                    CategoryName = course.Category?.Name,
                    InstructorId = course.InstructorId,
                    InstructorName = course.Instructor?.Name
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CourseResponseDto>> CreateCourse([FromBody] CourseCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // ? DTO ? Entity
                var course = new Course
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    CategoryId = dto.CategoryId,
                    InstructorId = dto.InstructorId
                };

                var created = await _courseRepository.AddAsync(course);

                // ? Entity ? DTO
                var result = new CourseResponseDto
                {
                    Id = created.Id,
                    Title = created.Title,
                    Description = created.Description,
                    CategoryId = created.CategoryId,
                    CategoryName = created.Category?.Name,
                    InstructorId = created.InstructorId,
                    InstructorName = created.Instructor?.Name
                };

                return CreatedAtAction(nameof(GetCourse), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpdateDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // ? DTO ? Entity
                var course = new Course
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    CategoryId = dto.CategoryId,
                    InstructorId = dto.InstructorId
                };

                var updated = await _courseRepository.UpdateAsync(course);
                if (updated == null)
                    return NotFound(new { message = $"Course with id {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var result = await _courseRepository.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Course with id {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
