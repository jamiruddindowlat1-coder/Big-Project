using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)  // ✅ Instructor ও load হবে
                .ToListAsync();
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)  // ✅ Instructor ও load হবে
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course> GetByNameAsync(string name)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Course> AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // ✅ Save এর পর Category এবং Instructor reload করো
            await _context.Entry(course).Reference(c => c.Category).LoadAsync();
            await _context.Entry(course).Reference(c => c.Instructor).LoadAsync();

            return course;
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            var existing = await GetByIdAsync(course.Id);
            if (existing == null) return null!;

            // ✅ Existing entity update করো (detach সমস্যা এড়াতে)
            existing.Title = course.Title;
            existing.Description = course.Description;
            existing.CategoryId = course.CategoryId;
            existing.InstructorId = course.InstructorId;

            await _context.SaveChangesAsync();

            // ✅ Update এর পর reload
            await _context.Entry(existing).Reference(c => c.Category).LoadAsync();
            await _context.Entry(existing).Reference(c => c.Instructor).LoadAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await GetByIdAsync(id);
            if (course == null) return false;
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}