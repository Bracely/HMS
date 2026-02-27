using System;
using System.Collections.Generic;
using System.Linq;
using HMS.Models;

namespace HMS.Services
{
    // ClinicService holds in-memory lists and simple business logic.
    public class ClinicService
    {
        private static readonly Lazy<ClinicService> _instance = new(() => new ClinicService());
        public static ClinicService Instance => _instance.Value;

        private readonly List<Patient> _patients = new();
        private readonly List<Doctor> _doctors = new();
        private readonly List<Appointment> _appointments = new();
        private readonly List<Bill> _bills = new();

        private int _patientId = 1;
        private int _doctorId = 1;
        private int _appointmentId = 1;
        private int _billId = 1;

        private ClinicService()
        {
            // Seed with some sample data for demonstration
            var d1 = AddDoctor(new Doctor { Name = "Dr. Alice Smith", Specialization = "General" });
            var d2 = AddDoctor(new Doctor { Name = "Dr. Bob Jones", Specialization = "Dentistry" });

            AddPatient(new Patient { FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 5, 1), PhoneNumber = "555-0100" });
            AddPatient(new Patient { FirstName = "Jane", LastName = "Roe", DateOfBirth = new DateTime(1985, 3, 12), PhoneNumber = "555-0123" });
        }

        // Patient operations
        public Patient AddPatient(Patient p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (string.IsNullOrWhiteSpace(p.FirstName) || string.IsNullOrWhiteSpace(p.LastName))
                throw new ArgumentException("Patient first and last name are required.");
            if (p.DateOfBirth > DateTime.Today) throw new ArgumentException("Date of birth cannot be in the future.");

            p.Id = _patientId++;
            _patients.Add(p);
            return p;
        }
        // Update an existing patient. Throws if not found or invalid.
        public Patient UpdatePatient(Patient p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            var existing = _patients.FirstOrDefault(x => x.Id == p.Id);
            if (existing == null) throw new InvalidOperationException("Patient not found.");
            if (string.IsNullOrWhiteSpace(p.FirstName) || string.IsNullOrWhiteSpace(p.LastName))
                throw new ArgumentException("Patient first and last name are required.");
            if (p.DateOfBirth > DateTime.Today) throw new ArgumentException("Date of birth cannot be in the future.");

            existing.FirstName = p.FirstName;
            existing.LastName = p.LastName;
            existing.DateOfBirth = p.DateOfBirth;
            existing.PhoneNumber = p.PhoneNumber;
            return existing;
        }

        // Delete a patient by id. Returns true if removed.
        public bool DeletePatient(int id)
        {
            var existing = _patients.FirstOrDefault(x => x.Id == id);
            if (existing == null) return false;
            return _patients.Remove(existing);
        }

        public List<Patient> GetPatients() => _patients.ToList();

        public List<Patient> SearchPatients(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return GetPatients();
            q = q.Trim().ToLowerInvariant();
            return _patients.Where(p => p.Id.ToString() == q
                || p.FirstName.ToLowerInvariant().Contains(q)
                || p.LastName.ToLowerInvariant().Contains(q)
                || (p.PhoneNumber ?? "").Contains(q)).ToList();
        }

        // Doctor operations
        public Doctor AddDoctor(Doctor d)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            if (string.IsNullOrWhiteSpace(d.Name)) throw new ArgumentException("Doctor name is required.");
            d.Id = _doctorId++;
            _doctors.Add(d);
            return d;
        }

        public List<Doctor> GetDoctors() => _doctors.ToList();

        // Update an existing doctor
        public Doctor UpdateDoctor(Doctor d)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            var existing = _doctors.FirstOrDefault(x => x.Id == d.Id);
            if (existing == null) throw new InvalidOperationException("Doctor not found.");
            if (string.IsNullOrWhiteSpace(d.Name)) throw new ArgumentException("Doctor name is required.");
            existing.Name = d.Name;
            existing.Specialization = d.Specialization;
            return existing;
        }

        public bool DeleteDoctor(int id)
        {
            var existing = _doctors.FirstOrDefault(x => x.Id == id);
            if (existing == null) return false;
            return _doctors.Remove(existing);
        }

        // Appointment operations
        public Appointment AddAppointment(Appointment a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (a.Patient == null || a.Doctor == null) throw new ArgumentException("Appointment must have a patient and a doctor.");
            if (a.Date < DateTime.Now.AddMinutes(-5)) throw new ArgumentException("Appointment date must be in the future or present.");
            a.AppointmentId = _appointmentId++;
            _appointments.Add(a);
            return a;
        }

        public List<Appointment> GetAppointments() => _appointments.ToList();

        // Update an appointment
        public Appointment UpdateAppointment(Appointment a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            var existing = _appointments.FirstOrDefault(x => x.AppointmentId == a.AppointmentId);
            if (existing == null) throw new InvalidOperationException("Appointment not found.");
            if (a.Patient == null || a.Doctor == null) throw new ArgumentException("Appointment must have a patient and a doctor.");
            if (a.Date < DateTime.Now.AddMinutes(-5)) throw new ArgumentException("Appointment date must be in the future or present.");

            existing.Patient = a.Patient;
            existing.Doctor = a.Doctor;
            existing.Date = a.Date;
            existing.Reason = a.Reason;
            return existing;
        }

        public bool DeleteAppointment(int id)
        {
            var existing = _appointments.FirstOrDefault(x => x.AppointmentId == id);
            if (existing == null) return false;
            return _appointments.Remove(existing);
        }

        // Billing operations
        public Bill AddBill(Bill b)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (b.Patient == null) throw new ArgumentException("Bill must have a patient.");
            if (b.Amount <= 0) throw new ArgumentException("Amount must be positive.");
            b.BillId = _billId++;
            _bills.Add(b);
            return b;
        }

        public List<Bill> GetBills() => _bills.ToList();

        // Update a bill
        public Bill UpdateBill(Bill b)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            var existing = _bills.FirstOrDefault(x => x.BillId == b.BillId);
            if (existing == null) throw new InvalidOperationException("Bill not found.");
            if (b.Patient == null) throw new ArgumentException("Bill must have a patient.");
            if (b.Amount <= 0) throw new ArgumentException("Amount must be positive.");

            existing.Patient = b.Patient;
            existing.Amount = b.Amount;
            existing.Date = b.Date;
            return existing;
        }

        public bool DeleteBill(int id)
        {
            var existing = _bills.FirstOrDefault(x => x.BillId == id);
            if (existing == null) return false;
            return _bills.Remove(existing);
        }
    }
}
