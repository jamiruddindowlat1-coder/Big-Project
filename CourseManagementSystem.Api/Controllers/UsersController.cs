using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UsersController(IUserRepository repo)
        {
            _repo = repo;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repo.GetAllAsync();
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.AddAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid || id != dto.Id)
                return BadRequest();

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.Role = dto.Role;

            // ✅ FIX: Email শুধু তখনই update করো যখন সত্যিই পরিবর্তন হয়েছে
            // Same email আবার set করলে unique index constraint error আসে
            if (!string.IsNullOrEmpty(dto.Email) &&
                !string.Equals(existing.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                // ✅ নতুন email আগে থেকে অন্য কেউ ব্যবহার করছে কিনা check করো
                var emailExists = await _repo.GetByEmailAsync(dto.Email);
                if (emailExists != null && emailExists.Id != id)
                    return Conflict(new { message = $"Email '{dto.Email}' is already in use by another user." });

                existing.Email = dto.Email;
            }

            // ✅ Password শুধু তখনই update করো যখন দেওয়া হয়েছে
            if (!string.IsNullOrEmpty(dto.Password))
            {
                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            // ✅ Optional fields — null হলে পুরনো value রাখো
            if (!string.IsNullOrEmpty(dto.Status))   existing.Status   = dto.Status;
            if (!string.IsNullOrEmpty(dto.Phone))     existing.Phone    = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Address))   existing.Address  = dto.Address;
            if (!string.IsNullOrEmpty(dto.Nid))       existing.Nid      = dto.Nid;
            if (!string.IsNullOrEmpty(dto.Expertise)) existing.Expertise = dto.Expertise;
            if (!string.IsNullOrEmpty(dto.ProfilePicture)) existing.ProfilePicture = dto.ProfilePicture;
            if (dto.Salary.HasValue)   existing.Salary   = dto.Salary.Value;
            if (dto.JoinDate.HasValue) existing.JoinDate = dto.JoinDate.Value;

            await _repo.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}