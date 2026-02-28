using CourseManagementSystem.Core.Models.Entities;
namespace CourseManagementSystem.Core.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course> GetByIdAsync(int id);
        Task<Course> GetByNameAsync(string name);
        Task<Course> AddAsync(Course course);      // ✅ Task<Course>
        Task<Course> UpdateAsync(Course course);   // ✅ Task<Course>
        Task<bool> DeleteAsync(int id);            // ✅ Task<bool>
    }
}