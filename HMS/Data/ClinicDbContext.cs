using Microsoft.EntityFrameworkCore;
using HMS.Models;
using System;

namespace HMS.Data
{
    public class ClinicDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<IllnessRecord> IllnessRecords { get; set; }

        private readonly string _dbPath;

        public ClinicDbContext()
        {
            // Default location next to application executable
            var folder = AppContext.BaseDirectory;
            _dbPath = System.IO.Path.Combine(folder, "hms.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure keys where names don't match conventions
            modelBuilder.Entity<Appointment>().HasKey(a => a.AppointmentId);
            modelBuilder.Entity<Bill>().HasKey(b => b.BillId);

            // Relationships: set cascade behavior as appropriate
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Patient)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IllnessRecord>()
                .HasOne(r => r.Patient)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IllnessRecord>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
