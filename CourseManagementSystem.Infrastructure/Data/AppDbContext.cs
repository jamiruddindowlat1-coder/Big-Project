using CourseManagementSystem.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AccountTransaction> Accounts { get; set; }

        // ✅ আগে থেকে ছিল
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Installment> Installments { get; set; }

        // ✅ নতুন যোগ
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── আগে থেকে ছিল — কোনো পরিবর্তন নেই ──────────────

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Fee).HasColumnType("decimal(18,2)");

                entity.HasOne(c => c.Category)
                      .WithMany(cat => cat.Courses)
                      .HasForeignKey(c => c.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Instructor)
                      .WithMany(u => u.Courses)
                      .HasForeignKey(c => c.InstructorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EnrollmentDate).IsRequired();

                entity.HasOne(e => e.Student)
                      .WithMany(u => u.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AccountTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.RelatedEntity).HasMaxLength(200);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DownPayment).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EnrollmentFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(p => p.Student)
                      .WithMany()
                      .HasForeignKey(p => p.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Course)
                      .WithMany()
                      .HasForeignKey(p => p.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Installment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Note).HasMaxLength(200);

                entity.HasOne(i => i.Payment)
                      .WithMany(p => p.Installments)
                      .HasForeignKey(i => i.PaymentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── নতুন যোগ ─────────────────────────────────────────

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DueDate).IsRequired();

                entity.HasOne(a => a.Course)
                      .WithMany()
                      .HasForeignKey(a => a.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Teacher)
                      .WithMany()
                      .HasForeignKey(a => a.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Student)
                      .WithMany()
                      .HasForeignKey(a => a.StudentId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AssignmentSubmission>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(s => s.Assignment)
                      .WithMany(a => a.Submissions)
                      .HasForeignKey(s => s.AssignmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Student)
                      .WithMany()
                      .HasForeignKey(s => s.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ClassSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Batch).HasMaxLength(100);
                entity.Property(e => e.RoomNo).HasMaxLength(100);
                entity.Property(e => e.Note).HasMaxLength(500);

                entity.HasOne(s => s.Course)
                      .WithMany()
                      .HasForeignKey(s => s.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}