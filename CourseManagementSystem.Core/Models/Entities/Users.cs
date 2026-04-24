#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseManagementSystem.Core.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Staff fields
        public string? Status { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Nid { get; set; }
        public string? Expertise { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? JoinDate { get; set; }

        [JsonIgnore]
        public ICollection<Course> Courses { get; set; } = new List<Course>();

        [JsonIgnore]
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}