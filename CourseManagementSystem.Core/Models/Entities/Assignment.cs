using System;
using System.Collections.Generic;

namespace CourseManagementSystem.Core.Models.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int TotalMarks { get; set; } = 100;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public int? StudentId { get; set; }

        // Navigation
        public Course Course { get; set; } = null!;
        public User Teacher { get; set; } = null!;
        public User? Student { get; set; }
        public ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
    }

    public class AssignmentSubmission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string SubmissionUrl { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = "Submitted";
        public int MarksObtained { get; set; } = 0;
        public string Feedback { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public Assignment Assignment { get; set; } = null!;
        public User Student { get; set; } = null!;
    }
}