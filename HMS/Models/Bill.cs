using System;
using HMS.Models;

namespace HMS.Models
{
    // Simple billing model
    public class Bill
    {
        public int BillId { get; set; }
        public Patient Patient { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public override string ToString() => $"{BillId}: {Patient?.FullName} - {Amount:C} on {Date:d}";
    }
}
