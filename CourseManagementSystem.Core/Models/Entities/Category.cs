using System.Collections.Generic;

namespace CourseManagementSystem.Core.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // One-to-Many: Category -> Courses
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}