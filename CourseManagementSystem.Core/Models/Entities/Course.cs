using System;
using System.Collections.Generic;

namespace CourseManagementSystem.Core.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }  
        public string Description { get; set; }

        // Relationship: Course -> Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Relationship: Course -> Instructor
        public int InstructorId { get; set; }
        public User Instructor { get; set; }

        // Relationship: Course -> Enrollments
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}