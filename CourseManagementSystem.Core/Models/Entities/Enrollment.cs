using System;
using System.Text.Json.Serialization;

namespace CourseManagementSystem.Core.Models.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        // Navigation properties
        [JsonIgnore]
        public User Student { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }
    }
}