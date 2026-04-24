namespace CourseManagementSystem.Core.Models.Entities
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        // "info" | "warning" | "important"
        public string Type { get; set; } = "info";

        // "all" | "student" | "teacher" | "staff"
        public string TargetAudience { get; set; } = "all";

        public bool IsActive { get; set; } = true;

        // Optional metadata
        public string? BatchNo { get; set; }
        public string? RoomNo { get; set; }
        public string? Subject { get; set; }
        public int? CourseId { get; set; }
        public DateTime? ClassDateTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
