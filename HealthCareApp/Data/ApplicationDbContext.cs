using HealthCareApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthCareApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Specialization> Specializations { get; set; }
        public virtual DbSet<SubSpecialization> SubSpecializations { get; set; }
        public virtual DbSet<Clinic> Clinics { get; set; }
        public virtual DbSet<Availability> Availability { get; set; }
        public virtual DbSet<AvailabilitySlots> AvailabilitySlots { get; set; }
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Notification relationship
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Doctor - Specialization relationship
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialization)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Doctor - SubSpecialization many-to-many relationship
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.SubSpecializations)
                .WithMany(s => s.Doctors);

            // Configure Review relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Patient)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Clinic relationship
            modelBuilder.Entity<Clinic>()
                .HasOne(c => c.Doctor)
                .WithMany(d => d.Clinics)
                .HasForeignKey(c => c.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Availability relationships
            modelBuilder.Entity<Availability>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.availabilities)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Availability>()
                .HasOne(a => a.Clinic)
                .WithMany(c => c.Availabilities)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure AvailabilitySlots relationship
            modelBuilder.Entity<AvailabilitySlots>()
                .HasOne(a => a.Availability)
                .WithMany(a => a.AvailableSlots)
                .HasForeignKey(a => a.AvailabilityId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Appointment relationship with Patient
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Appointment relationship with AvailabilitySlots
            //modelBuilder.Entity<Appointment>()
            //    .HasOne(a => a.AvailableSlot)
            //    .WithOne(s => s.Appointment)
            //    .HasForeignKey<Appointment>(a => a.SlotId)
            //    .OnDelete(DeleteBehavior.NoAction);

            // Configure circular reference between Appointment and Payment
            //modelBuilder.Entity<Appointment>()
            //    .HasOne(a => a.Payment)
            //    .WithOne(p => p.Appointment)
            //    .HasForeignKey<Appointment>(a => a.PaymentId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Payment>()
            //    .HasOne(p => p.Appointment)
            //    .WithOne()
            //    .HasForeignKey<Payment>(p => p.AppointmentId)
            //    .OnDelete(DeleteBehavior.NoAction);

            // Configure MedicalRecord relationship
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure SubSpecialization relationship with Specialization
            modelBuilder.Entity<SubSpecialization>()
                .HasOne(s => s.Specialization)
                .WithMany(s => s.SubSpecialization)
                .HasForeignKey(s => s.SpecializationId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
