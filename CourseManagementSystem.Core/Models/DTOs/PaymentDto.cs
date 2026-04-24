namespace CourseManagementSystem.Core.DTOs
{
    public class InstallmentDto
    {
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
    }

    public class PaymentCreateDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
        public decimal DownPayment { get; set; }
        public decimal EnrollmentFee { get; set; }
        public List<InstallmentDto> Installments { get; set; } = new();
    }

    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal Amount { get; set; }
        public decimal DownPayment { get; set; }
        public decimal EnrollmentFee { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InstallmentDto> Installments { get; set; } = new();
    }
}