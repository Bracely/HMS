using System;
using HMS.Models;

namespace HMS.Models
{
    // Appointment model ties patient and doctor together
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }

        public override string ToString() => $"{AppointmentId}: {Patient?.FullName} with {Doctor?.Name} on {Date}";
    }
}
