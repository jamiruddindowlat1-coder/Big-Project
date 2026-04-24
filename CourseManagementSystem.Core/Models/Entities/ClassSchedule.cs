namespace CourseManagementSystem.Core.Models.Entities
{
    public class ClassSchedule
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Batch { get; set; } = string.Empty;
        public string RoomNo { get; set; } = string.Empty;
        public DateTime ClassDateTime { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Course Course { get; set; } = null!;
    }
}
