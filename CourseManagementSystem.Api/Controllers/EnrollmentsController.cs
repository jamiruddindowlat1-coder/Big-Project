using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/Enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        // Static list - 
        private static List<string> enrollments = new List<string>
        {
            "Course1",
            "Course2"
        };

        // ✅ GET ALL — GET /api/Enrollments
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            return Ok(enrollments);
        }

        // ✅ CREATE — POST /api/Enrollments
        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] string enrollment)
        {
            if (string.IsNullOrWhiteSpace(enrollment))
                return BadRequest(new { message = "Enrollment name is required!" });

            enrollments.Add(enrollment);

            return Ok(new { message = "Enrollment created!" });
        }

        // ✅ UPDATE — PUT /api/Enrollments/{index}
        [HttpPut("{index}")]
        [Authorize]
        public IActionResult Update(int index, [FromBody] string updatedEnrollment)
        {
            if (string.IsNullOrWhiteSpace(updatedEnrollment))
                return BadRequest(new { message = "Enrollment name is required!" });

            if (index < 0 || index >= enrollments.Count)
                return NotFound(new { message = "Enrollment not found!" });

            enrollments[index] = updatedEnrollment;

            return Ok(new { message = $"Enrollment updated to '{updatedEnrollment}'!" });
        }

        // ✅ DELETE — DELETE /api/Enrollments/{index}
        [HttpDelete("{index}")]
        [Authorize]
        public IActionResult Delete(int index)
        {
            if (index < 0 || index >= enrollments.Count)
                return NotFound(new { message = "Enrollment not found!" });

            var removed = enrollments[index];
            enrollments.RemoveAt(index);

            return Ok(new { message = $"Enrollment '{removed}' deleted successfully!" });
        }
    }
}