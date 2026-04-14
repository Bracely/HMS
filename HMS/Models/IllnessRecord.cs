using System;

namespace HMS.Models
{
    // Represents a doctor's logged illness/encounter for a patient
    public class IllnessRecord
    {
        public int Id { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime Date { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
    }
}
