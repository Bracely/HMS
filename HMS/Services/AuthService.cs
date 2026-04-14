using System;
using HMS.Models;

namespace HMS.Services
{
    public enum UserRole { None, Admin, Doctor, Student }

    // Simple authentication/authorization service for in-memory demo
    //
    // Responsibilities:
    // - Track the currently authenticated principal and role for the
    //   running process (single-process demo). This is intentionally
    //   simple and not secure: it is suitable for development and demo
    //   scenarios where a full identity stack is not required.
    // - Provide convenience login methods for Doctor, Student and Admin
    //   that operate against the in-memory ClinicService data.
    //
    // Notes about production readiness:
    // - Do NOT use this approach in production. Replace with a robust
    //   authentication layer (ASP.NET Identity, OAuth2, JWT, etc.) and
    //   a persistent user store.
    public static class AuthService
    {
        public static UserRole CurrentRole { get; private set; } = UserRole.None;
        public static Doctor CurrentDoctor { get; private set; }
        public static Patient CurrentPatient { get; private set; }
        // Optional admin name for display
        public static string? AdminName { get; private set; }

        public static void Logout()
        {
            CurrentRole = UserRole.None;
            CurrentDoctor = null;
            CurrentPatient = null;
        }

        // Login as admin using a password. Password is read from an environment variable
        // HMS_ADMIN_PASSWORD. If not set, default password is "admin" (development only).
        public static bool LoginAdmin(string password)
        {
            var configured = Environment.GetEnvironmentVariable("HMS_ADMIN_PASSWORD");
            if (string.IsNullOrEmpty(configured)) configured = "admin"; // default for local/dev
            if (password == null) return false;
            if (password == configured)
            {
                CurrentRole = UserRole.Admin;
                CurrentDoctor = null;
                CurrentPatient = null;
                AdminName = "Administrator";
                return true;
            }
            return false;
        }

        // Login by doctor email
        public static bool LoginDoctor(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var doc = ClinicService.Instance.GetDoctors().Find(d => string.Equals(d.Email?.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));
            if (doc == null) return false;
            CurrentDoctor = doc;
            CurrentRole = UserRole.Doctor;
            CurrentPatient = null;
            return true;
        }

        // Login by student id
        public static bool LoginStudent(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) return false;
            var pat = ClinicService.Instance.GetPatients().Find(p => string.Equals(p.StudentId?.Trim(), studentId.Trim(), StringComparison.OrdinalIgnoreCase));
            if (pat == null) return false;
            CurrentPatient = pat;
            CurrentRole = UserRole.Student;
            CurrentDoctor = null;
            return true;
        }
    }
}
