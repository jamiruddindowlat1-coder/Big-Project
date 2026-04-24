#nullable enable
using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.DTOs
{
    public class UserCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Student";

        public string? Status { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Nid { get; set; }
        public string? Expertise { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? JoinDate { get; set; }
    }

    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Nid { get; set; }
        public string? Expertise { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? JoinDate { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Nid { get; set; }
        public string? Expertise { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? JoinDate { get; set; }
    }
}