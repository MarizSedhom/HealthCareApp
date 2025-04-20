using System.ComponentModel;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DoctorIdxPendingVM
    {

        [DisplayName("Doctor Name")]
        public string DrName { get; set; }
      
        public Title Title { get; set; }
        public string Specialization { get; set; }

        [DisplayName("Experience Years")]
        public int ExperienceYears { get; set; }

        [DisplayName("Registered On")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Verification File")]
        public string verificationFileName { get; set; } 
        public string doctorId { get; set; }
    }
}
