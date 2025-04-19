using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Doctor:ApplicationUser
    {
        public Title Title { get; set; }
        public string Description { get; set; }
        public decimal Fees { get; set; }
        public int ExperienceYears { get; set; }
        public int WaitingTimeInMinutes { get; set; }
        public string ProfilePicture {  get; set; }
        public VerificationStatus verificationStatus { get; set; }
        public string verificationFileName { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }=new List<Review>();
        public virtual ICollection<Clinic>? Clinics { get; set; } =new List<Clinic>();
        public virtual ICollection<Availability>? availabilities { get; set; }=new List<Availability>();
        public virtual ICollection<SubSpecialization>? SubSpecializations { get; set; }=new List<SubSpecialization>();
        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; } = new List<MedicalRecord>();
       
        [ForeignKey("Specialization")]
        public int SpecializationId { get; set; }
        public virtual Specialization? Specialization { get; set; }


    }
    public enum VerificationStatus
    {
        Accepted,
        Rejected,
        Pinding
    }

    public enum Title
    {
        Professor,
        Lecturer,
        Consultant,
        Specialist
    }
}
