#nullable enable
using System;
using System.Collections.Generic;

namespace CourseManagementSystem.Core.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Duration { get; set; }
        public int CategoryId { get; set; }
        public int InstructorId { get; set; }
        public decimal Fee { get; set; }
        public bool AllowInstallment { get; set; } = false;
        public int NumberOfInstallments { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Category? Category { get; set; }
        public User? Instructor { get; set; }
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}