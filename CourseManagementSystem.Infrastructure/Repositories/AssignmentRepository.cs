using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Core.Models.Entities;
using CourseManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Infrastructure.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ApplicationDbContext _db;

        public AssignmentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // ── Assignment ────────────────────────────────

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _db.Assignments
                .Include(a => a.Course)
                .Include(a => a.Teacher)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _db.Assignments
                .Include(a => a.Course)
                .Include(a => a.Teacher)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Assignment>> GetByTeacherIdAsync(int teacherId)
        {
            return await _db.Assignments
                .Include(a => a.Course)
                .Where(a => a.TeacherId == teacherId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetByCourseIdAsync(int courseId)
        {
            return await _db.Assignments
                .Include(a => a.Teacher)
                .Where(a => a.CourseId == courseId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Assignment> AddAsync(Assignment assignment)
        {
            _db.Assignments.Add(assignment);
            await _db.SaveChangesAsync();
            // Navigation properties load করো
            await _db.Entry(assignment).Reference(a => a.Course).LoadAsync();
            await _db.Entry(assignment).Reference(a => a.Teacher).LoadAsync();
            return assignment;
        }

        public async Task<Assignment?> UpdateAsync(Assignment assignment)
        {
            var existing = await _db.Assignments.FindAsync(assignment.Id);
            if (existing == null) return null;

            existing.Title       = assignment.Title;
            existing.Description = assignment.Description;
            existing.DueDate     = assignment.DueDate;
            existing.TotalMarks  = assignment.TotalMarks;
            existing.CourseId    = assignment.CourseId;
            existing.TeacherId   = assignment.TeacherId;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _db.Assignments.FindAsync(id);
            if (assignment == null) return false;

            _db.Assignments.Remove(assignment);
            await _db.SaveChangesAsync();
            return true;
        }

        // ── Submission ────────────────────────────────

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            return await _db.AssignmentSubmissions
                .Include(s => s.Student)
                .Where(s => s.AssignmentId == assignmentId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByStudentIdAsync(int studentId)
        {
            return await _db.AssignmentSubmissions
                .Include(s => s.Assignment)
                    .ThenInclude(a => a!.Course)
                .Where(s => s.StudentId == studentId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<AssignmentSubmission?> GetSubmissionAsync(int assignmentId, int studentId)
        {
            return await _db.AssignmentSubmissions
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);
        }

        public async Task<AssignmentSubmission?> GetSubmissionByIdAsync(int submissionId)
        {
            return await _db.AssignmentSubmissions
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.Id == submissionId);
        }

        public async Task<AssignmentSubmission> AddSubmissionAsync(AssignmentSubmission submission)
        {
            _db.AssignmentSubmissions.Add(submission);
            await _db.SaveChangesAsync();
            await _db.Entry(submission).Reference(s => s.Student).LoadAsync();
            return submission;
        }

        public async Task<AssignmentSubmission?> UpdateSubmissionAsync(AssignmentSubmission submission)
        {
            var existing = await _db.AssignmentSubmissions.FindAsync(submission.Id);
            if (existing == null) return null;

            existing.Status        = submission.Status;
            existing.MarksObtained = submission.MarksObtained;
            existing.Feedback      = submission.Feedback;

            await _db.SaveChangesAsync();
            return existing;
        }
    }
}