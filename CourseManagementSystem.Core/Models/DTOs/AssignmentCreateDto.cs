#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.DTOs
{
    // ── Assignment DTOs ──────────────────────────────

    public class AssignmentCreateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int TotalMarks { get; set; } = 100;

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int TeacherId { get; set; }

        public int? StudentId { get; set; }
    }

    public class AssignmentUpdateDto : AssignmentCreateDto
    {
        [Required]
        public int Id { get; set; }
    }

    public class AssignmentResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public int TotalMarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int? StudentId { get; set; }
        public string? StudentName { get; set; }
    }

    // ── Submission DTOs ──────────────────────────────

    public class SubmissionCreateDto
    {
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string? SubmissionUrl { get; set; }
        public string? Content { get; set; }
    }

    public class GradeSubmissionDto
    {
        public int Marks { get; set; }
        public string? Feedback { get; set; }
    }

    public class SubmissionResponseDto
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? SubmissionUrl { get; set; }
        public string? Content { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? MarksObtained { get; set; }
        public string? Feedback { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}