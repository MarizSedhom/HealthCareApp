using HealthCareApp.Models;
using System.ComponentModel;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DoctorIdxVM
    {
        public string DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string drName {  get; set; }
        public string Title { get; set; }
        public string Specialization { get; set; }

        [DisplayName("Experience Years")]
        public int ExperienceYears { get; set; }

        [DisplayName("Status")]
        public VerificationStatus verificationStatus { get; set; }

    }
}
