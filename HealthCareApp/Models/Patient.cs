namespace HealthCareApp.Models
{
    public class Patient:ApplicationUser
    {
        public string EmergencyContact { get; set; }
        public string MedicalHistory { get; set; }

        public virtual ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Review>? Reviews { get; set; } = new List<Review>();
        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; } = new List<MedicalRecord>();

        public bool IsDeleted { get; set; } = false;
    }
}
