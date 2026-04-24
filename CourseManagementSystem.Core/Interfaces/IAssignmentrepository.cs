#nullable enable
using CourseManagementSystem.Core.Models.Entities;

namespace CourseManagementSystem.Core.Interfaces
{
    public interface IAssignmentRepository
    {
        // Assignment CRUD
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment?> GetByIdAsync(int id);
        Task<IEnumerable<Assignment>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<Assignment>> GetByCourseIdAsync(int courseId);
        Task<Assignment> AddAsync(Assignment assignment);
        Task<Assignment?> UpdateAsync(Assignment assignment);
        Task<bool> DeleteAsync(int id);

        // Submission
        Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByStudentIdAsync(int studentId);
        Task<AssignmentSubmission?> GetSubmissionAsync(int assignmentId, int studentId);
        Task<AssignmentSubmission> AddSubmissionAsync(AssignmentSubmission submission);
        Task<AssignmentSubmission?> UpdateSubmissionAsync(AssignmentSubmission submission);
        Task<AssignmentSubmission?> GetSubmissionByIdAsync(int submissionId);
    }
}