namespace CourseManagementSystem.Core.Models
{
    /// <summary>
    /// Result model for authentication operations
    /// </summary>
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}