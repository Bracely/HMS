using System;

namespace HMS.Models
{
    // Doctor model
    public class Doctor
    {
        // Auto-assigned by ClinicService
        public int Id { get; set; }
        public string Name { get; set; }
        // Contact email for doctor login
        public string Email { get; set; }
        public string Specialization { get; set; }

        public override string ToString() => $"{Id}: {Name} ({Specialization})";
    }
}
