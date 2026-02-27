using System;

namespace HMS.Models
{
    // Patient model represents a patient in the clinic
    public class Patient
    {
        // Auto-assigned by ClinicService
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }

        // Convenience property
        public string FullName => $"{FirstName} {LastName}";

        public override string ToString() => $"{Id}: {FullName}";
    }
}
