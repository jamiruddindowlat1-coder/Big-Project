#nullable enable
using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.DTOs
{
    public class CourseCreateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        public int Credits { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int InstructorId { get; set; }

        [Required]
        public decimal Fee { get; set; }

        public bool AllowInstallment { get; set; } = false;

        public int NumberOfInstallments { get; set; } = 1;
    }

    public class CourseUpdateDto : CourseCreateDto
    {
        [Required]
        public int Id { get; set; }
    }

    public class CourseResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Duration { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public bool AllowInstallment { get; set; }
        public int NumberOfInstallments { get; set; }
    }
}