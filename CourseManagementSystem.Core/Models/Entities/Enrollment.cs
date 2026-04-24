using System;
using System.Text.Json.Serialization;

namespace CourseManagementSystem.Core.Models.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        // ✅ Payment mapping fields for Enrollment Management
        public decimal TotalFee { get; set; }
        public decimal PaidAmount { get; set; }
        public int InstallmentCount { get; set; } = 1;
        public string PaymentMethod { get; set; } = "Cash"; // ✅ Cash, bKash, Nagad, Card, Bank
        public string PaymentStatus { get; set; } = "Unpaid";

        // Navigation properties
        [JsonIgnore]
        public User Student { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }
    }
}