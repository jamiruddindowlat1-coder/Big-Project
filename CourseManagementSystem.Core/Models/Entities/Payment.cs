using System.Collections.Generic;
namespace CourseManagementSystem.Core.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public User Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public decimal Amount { get; set; }
        public decimal DownPayment { get; set; }
        public decimal EnrollmentFee { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Installment> Installments { get; set; } = new List<Installment>();
    }

    public class Installment
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
    }
}