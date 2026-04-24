using System;

namespace CourseManagementSystem.Core.Models.Entities
{
    public class AccountTransaction
    {
        public int Id { get; set; }

        public string TransactionType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public string? Description { get; set; }
        public string? RelatedEntity { get; set; }

        public string PaymentMethod { get; set; } = "Cash"; // ✅ Cash, bKash, Nagad, Card, Bank
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}