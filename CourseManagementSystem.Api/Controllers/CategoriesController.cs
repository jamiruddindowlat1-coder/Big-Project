using Microsoft.AspNetCore.Mvc;
using CourseManagementSystem.Core.DTOs;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Core.Interfaces;

namespace CourseManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryRepository categoryRepository, ILogger<CategoriesController> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();

                
                var result = categories.Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    return NotFound(new { message = $"Category with id {id} not found" });

                // ✅ Entity → DTO
                var result = new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // ✅ DTO → Entity
                var category = new Category
                {
                    Name = dto.Name
                };

                var created = await _categoryRepository.AddAsync(category);

                // ✅ Entity → DTO
                var result = new CategoryResponseDto
                {
                    Id = created.Id,
                    Name = created.Name
                };

                return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // ✅ DTO → Entity
                var category = new Category
                {
                    Id = dto.Id,
                    Name = dto.Name
                };

                var updated = await _categoryRepository.UpdateAsync(category);
                if (updated == null)
                    return NotFound(new { message = $"Category with id {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryRepository.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Category with id {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}