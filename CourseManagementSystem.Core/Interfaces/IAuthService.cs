using CourseManagementSystem.Core.Models;
using CourseManagementSystem.Core.Models.Entities;

namespace CourseManagementSystem.Core.Interfaces
{
    public interface IAuthService
    {
        // Main authentication methods - returns AuthResult with token
        Task<AuthResult> RegisterAsync(string username, string email, string password);
        Task<AuthResult> LoginAsync(string username, string password);

        // Helper methods
        Task<bool> UserExistsAsync(string email);
        Task<User> GetUserByEmailAsync(string email);
        string GenerateJwtToken(User user);
    }
}