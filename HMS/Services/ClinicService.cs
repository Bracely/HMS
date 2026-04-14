using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services
{
    // ClinicService holds in-memory collections and implements simple
    // business logic used by the demo application. It is the central
    // in-process data store for Patients, Doctors, Appointments, Bills
    // and IllnessRecords. All operations are synchronous and operate on
    // lists kept in memory. This avoids external dependencies and keeps
    // the sample easy to run, but means data will not persist across
    // application restarts.
    //
    // Responsibilities:
    // - Provide CRUD-style operations for domain entities.
    // - Enforce simple validation rules (e.g. student ID format,
    //   uniqueness constraints).
    // - Provide aggregate queries used by dashboards and reports.
    //
    // Considerations:
    // - For real applications migrate this logic to a repository using
    //   EF Core or another persistent store and introduce proper
    //   transaction and concurrency handling.
    public class ClinicService
    {
        private static readonly Lazy<ClinicService> _instance = new(() => new ClinicService());
        public static ClinicService Instance => _instance.Value;

        private readonly Data.ClinicDbContext _db;
        // default appointment slot length in minutes
        private readonly int _appointmentSlotMinutes = 30;

        private ClinicService()
        {
            _db = new Data.ClinicDbContext();
            // create DB if needed
            _db.Database.EnsureCreated();

            // Seed with some sample data if empty
            if (!_db.Doctors.Any())
            {
                _db.Doctors.AddRange(new Doctor { Name = "Dr. Alice Smith", Specialization = "General", Email = "alice.smith@hit.ac.zw" },
                    new Doctor { Name = "Dr. Bob Jones", Specialization = "Dentistry", Email = "bob.jones@hit.ac.zw" });
                _db.SaveChanges();
            }

            if (!_db.Patients.Any())
            {
                _db.Patients.AddRange(new Patient { FirstName = "John", LastName = "Doe", StudentId = "H240101A", DateOfBirth = new DateTime(1990, 5, 1), PhoneNumber = "555-0100" },
                    new Patient { FirstName = "Jane", LastName = "Roe", StudentId = "H230212B", DateOfBirth = new DateTime(1985, 3, 12), PhoneNumber = "555-0123" });
                _db.SaveChanges();
            }
        }

        // Patient operations
        public Patient AddPatient(Patient p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (string.IsNullOrWhiteSpace(p.FirstName) || string.IsNullOrWhiteSpace(p.LastName))
                throw new ArgumentException("Patient first and last name are required.");
            if (p.DateOfBirth > DateTime.Today) throw new ArgumentException("Date of birth cannot be in the future.");

            // Validate optional student id (format like H250504N)
            if (!string.IsNullOrWhiteSpace(p.StudentId))
            {
                if (!Regex.IsMatch(p.StudentId.Trim(), "^H\\d{6}[A-Z]$", RegexOptions.IgnoreCase))
                    throw new ArgumentException("Student ID must be in the format H######X (e.g. H250504N).");
                // Ensure uniqueness
                if (_db.Patients.Any(x => !string.IsNullOrWhiteSpace(x.StudentId) && string.Equals(x.StudentId.Trim(), p.StudentId.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException("Student ID already exists.");
            }

            _db.Patients.Add(p);
            _db.SaveChanges();
            return p;
        }
        // Update an existing patient. Throws if not found or invalid.
        public Patient UpdatePatient(Patient p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            var existing = _db.Patients.FirstOrDefault(x => x.Id == p.Id);
            if (existing == null) throw new InvalidOperationException("Patient not found.");
            if (string.IsNullOrWhiteSpace(p.FirstName) || string.IsNullOrWhiteSpace(p.LastName))
                throw new ArgumentException("Patient first and last name are required.");
            if (p.DateOfBirth > DateTime.Today) throw new ArgumentException("Date of birth cannot be in the future.");

            // Validate optional student id
            if (!string.IsNullOrWhiteSpace(p.StudentId))
            {
                if (!Regex.IsMatch(p.StudentId.Trim(), "^H\\d{6}[A-Z]$", RegexOptions.IgnoreCase))
                    throw new ArgumentException("Student ID must be in the format H######X (e.g. H250504N).");
                // Ensure uniqueness among other patients
                if (_db.Patients.Any(x => x.Id != p.Id && !string.IsNullOrWhiteSpace(x.StudentId) && string.Equals(x.StudentId.Trim(), p.StudentId.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException("Student ID already exists.");
            }

            existing.FirstName = p.FirstName;
            existing.LastName = p.LastName;
            existing.StudentId = p.StudentId;
            existing.DateOfBirth = p.DateOfBirth;
            existing.PhoneNumber = p.PhoneNumber;
            _db.SaveChanges();
            return existing;
        }

        // Delete a patient by id. Returns true if removed.
        public bool DeletePatient(int id)
        {
            var existing = _db.Patients.FirstOrDefault(x => x.Id == id);
            if (existing == null) return false;
            _db.Patients.Remove(existing);
            _db.SaveChanges();
            return true;
        }

        public List<Patient> GetPatients() => _db.Patients.ToList();

        public List<Patient> SearchPatients(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return GetPatients();
            q = q.Trim().ToLowerInvariant();
            return _db.Patients.Where(p => p.Id.ToString() == q
                || p.FirstName.ToLowerInvariant().Contains(q)
                || p.LastName.ToLowerInvariant().Contains(q)
                || (p.PhoneNumber ?? "").Contains(q)
                || ((p.StudentId ?? "").ToLowerInvariant().Contains(q))).ToList();
        }

        // Doctor operations
        public Doctor AddDoctor(Doctor d)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            if (string.IsNullOrWhiteSpace(d.Name)) throw new ArgumentException("Doctor name is required.");
            if (!string.IsNullOrWhiteSpace(d.Email))
            {
                if (_db.Doctors.Any(x => !string.IsNullOrWhiteSpace(x.Email) && string.Equals(x.Email.Trim(), d.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException("Doctor email already exists.");
            }
            _db.Doctors.Add(d);
            _db.SaveChanges();
            return d;
        }

        public List<Doctor> GetDoctors() => _db.Doctors.ToList();

        // Update an existing doctor
        public Doctor UpdateDoctor(Doctor d)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            var existing = _db.Doctors.FirstOrDefault(x => x.Id == d.Id);
            if (existing == null) throw new InvalidOperationException("Doctor not found.");
            if (string.IsNullOrWhiteSpace(d.Name)) throw new ArgumentException("Doctor name is required.");
            // ensure email uniqueness
            if (!string.IsNullOrWhiteSpace(d.Email))
            {
                if (_db.Doctors.Any(x => x.Id != d.Id && !string.IsNullOrWhiteSpace(x.Email) && string.Equals(x.Email.Trim(), d.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException("Doctor email already exists.");
            }
            existing.Name = d.Name;
            existing.Specialization = d.Specialization;
            existing.Email = d.Email;
            _db.SaveChanges();
            return existing;
        }

        public bool DeleteDoctor(int id)
        {
            var existing = _db.Doctors.FirstOrDefault(x => x.Id == id);
            if (existing == null) return false;
            _db.Doctors.Remove(existing);
            _db.SaveChanges();
            return true;
        }

        // Appointment operations
        public Appointment AddAppointment(Appointment a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (a.Patient == null || a.Doctor == null) throw new ArgumentException("Appointment must have a patient and a doctor.");
            if (a.Date < DateTime.Now.AddMinutes(-5)) throw new ArgumentException("Appointment date must be in the future or present.");

            // Check doctor's availability for this timeslot
            if (!IsDoctorAvailable(a.Doctor.Id, a.Date))
                throw new ArgumentException("The selected doctor is not available at the requested time.");

            _db.Appointments.Add(a);
            _db.SaveChanges();
            return a;
        }

        // Check availability: true if the doctor has no appointment within the slot window
        public bool IsDoctorAvailable(int doctorId, DateTime dateTime)
        {
            // normalize to minutes
            var slotStart = dateTime;
            var slotEnd = slotStart.AddMinutes(_appointmentSlotMinutes);

            var conflicts = _db.Appointments.Where(x => x.Doctor != null && x.Doctor.Id == doctorId)
                .Any(x => slotStart < x.Date.AddMinutes(_appointmentSlotMinutes) && x.Date < slotEnd);

            return !conflicts;
        }

        // Return available time slots for a given doctor on a specific date.
        // Slots are generated between startHour and endHour and exclude already booked slots.
        public System.Collections.Generic.List<DateTime> GetAvailableSlots(int doctorId, DateTime day, int startHour = 9, int endHour = 17)
        {
            var slots = new System.Collections.Generic.List<DateTime>();
            var date = day.Date;
            for (var hour = startHour; hour < endHour; hour++)
            {
                for (var minute = 0; minute < 60; minute += _appointmentSlotMinutes)
                {
                    var candidate = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                    if (candidate < DateTime.Now.AddMinutes(-5)) continue; // don't offer past slots
                    if (IsDoctorAvailable(doctorId, candidate)) slots.Add(candidate);
                }
            }
            return slots;
        }

        public List<Appointment> GetAppointments() => _db.Appointments.Include(a => a.Patient).Include(a => a.Doctor).ToList();

        // Illness record operations
        public IllnessRecord AddIllnessRecord(IllnessRecord r)
        {
            if (r == null) throw new ArgumentNullException(nameof(r));
            if (r.Patient == null) throw new ArgumentException("Illness record must have a patient.");
            if (r.Doctor == null) throw new ArgumentException("Illness record must have a doctor.");
            if (string.IsNullOrWhiteSpace(r.Diagnosis)) throw new ArgumentException("Diagnosis is required.");

            _db.IllnessRecords.Add(r);
            _db.SaveChanges();
            return r;
        }

        public List<IllnessRecord> GetIllnessRecords() => _db.IllnessRecords.Include(r => r.Patient).Include(r => r.Doctor).ToList();

        public List<IllnessRecord> GetIllnessRecordsForPatient(int patientId) => _db.IllnessRecords.Include(r => r.Patient).Include(r => r.Doctor).Where(x => x.Patient.Id == patientId).ToList();

        public List<IllnessRecord> GetIllnessRecordsForDoctor(int doctorId) => _db.IllnessRecords.Include(r => r.Patient).Include(r => r.Doctor).Where(x => x.Doctor.Id == doctorId).ToList();

        // Simple stats: most common diagnoses
        public Dictionary<string, int> GetDiagnosisStatistics()
        {
            return _db.IllnessRecords.ToList().GroupBy(r => (r.Diagnosis ?? string.Empty).Trim().ToLowerInvariant()).Where(g => !string.IsNullOrEmpty(g.Key)).ToDictionary(g => g.Key, g => g.Count());
        }

        // Update an illness record
        public IllnessRecord UpdateIllnessRecord(IllnessRecord r)
        {
            if (r == null) throw new ArgumentNullException(nameof(r));
            var existing = _db.IllnessRecords.FirstOrDefault(x => x.Id == r.Id);
            if (existing == null) throw new InvalidOperationException("Illness record not found.");
            if (r.Patient == null || r.Doctor == null) throw new ArgumentException("Illness record must have a patient and a doctor.");
            if (string.IsNullOrWhiteSpace(r.Diagnosis)) throw new ArgumentException("Diagnosis is required.");
            existing.Patient = r.Patient;
            existing.Doctor = r.Doctor;
            existing.Date = r.Date;
            existing.Diagnosis = r.Diagnosis;
            existing.Notes = r.Notes;
            _db.SaveChanges();
            return existing;
        }

        public bool DeleteIllnessRecord(int id)
        {
            var existing = _db.IllnessRecords.FirstOrDefault(x => x.Id == id);
            if (existing == null) return false;
            _db.IllnessRecords.Remove(existing);
            _db.SaveChanges();
            return true;
        }

        // Update an appointment
        public Appointment UpdateAppointment(Appointment a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            var existing = _db.Appointments.FirstOrDefault(x => x.AppointmentId == a.AppointmentId);
            if (existing == null) throw new InvalidOperationException("Appointment not found.");
            if (a.Patient == null || a.Doctor == null) throw new ArgumentException("Appointment must have a patient and a doctor.");
            if (a.Date < DateTime.Now.AddMinutes(-5)) throw new ArgumentException("Appointment date must be in the future or present.");

            existing.Patient = a.Patient;
            existing.Doctor = a.Doctor;
            existing.Date = a.Date;
            existing.Reason = a.Reason;
            _db.SaveChanges();
            return existing;
        }

        public bool DeleteAppointment(int id)
        {
            var existing = _db.Appointments.FirstOrDefault(x => x.AppointmentId == id);
            if (existing == null) return false;
            _db.Appointments.Remove(existing);
            _db.SaveChanges();
            return true;
        }

        // Billing operations
        public Bill AddBill(Bill b)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (b.Patient == null) throw new ArgumentException("Bill must have a patient.");
            if (b.Amount <= 0) throw new ArgumentException("Amount must be positive.");
            _db.Bills.Add(b);
            _db.SaveChanges();
            return b;
        }

        public List<Bill> GetBills() => _db.Bills.Include(b => b.Patient).ToList();

        // Update a bill
        public Bill UpdateBill(Bill b)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            var existing = _db.Bills.FirstOrDefault(x => x.BillId == b.BillId);
            if (existing == null) throw new InvalidOperationException("Bill not found.");
            if (b.Patient == null) throw new ArgumentException("Bill must have a patient.");
            if (b.Amount <= 0) throw new ArgumentException("Amount must be positive.");

            existing.Patient = b.Patient;
            existing.Amount = b.Amount;
            existing.Date = b.Date;
            _db.SaveChanges();
            return existing;
        }

        public bool DeleteBill(int id)
        {
            var existing = _db.Bills.FirstOrDefault(x => x.BillId == id);
            if (existing == null) return false;
            _db.Bills.Remove(existing);
            _db.SaveChanges();
            return true;
        }
    }
}
