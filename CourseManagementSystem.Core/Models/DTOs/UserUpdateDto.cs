using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}