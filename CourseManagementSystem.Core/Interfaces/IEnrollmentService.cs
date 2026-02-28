using CourseManagementSystem.Core.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseManagementSystem.Core.Interfaces
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync();
        Task<Enrollment> GetEnrollmentByIdAsync(int id);
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task UpdateEnrollmentAsync(Enrollment enrollment);
        Task DeleteEnrollmentAsync(int id);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(int userId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId);
        object UpdateEnrollment(Enrollment enrollment);
        object CreateEnrollment(Enrollment enrollment);
        object GetEnrollmentById(int id);
        object GetAllEnrollments();
    }
}