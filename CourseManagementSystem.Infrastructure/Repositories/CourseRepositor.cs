#nullable enable
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
                .Include(c => c.Instructor)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course?> GetByNameAsync(string name)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Title == name);
        }

        public async Task<Course> AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            await _context.Entry(course).Reference(c => c.Category).LoadAsync();
            await _context.Entry(course).Reference(c => c.Instructor).LoadAsync();

            return course;
        }

        public async Task<Course?> UpdateAsync(Course course)
        {
            var existing = await GetByIdAsync(course.Id);
            if (existing == null) return null;

            existing.Title = course.Title;
            existing.Description = course.Description;
            existing.CategoryId = course.CategoryId;
            existing.InstructorId = course.InstructorId;

            await _context.SaveChangesAsync();

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