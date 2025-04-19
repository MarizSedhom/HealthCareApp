using HealthCareApp.Models;
using HealthCareApp.ViewModel.Clinic;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DoctorDetailsVM
    {


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Fees { get; set; }
        public int ExperienceYears { get; set; }
        public int WaitingTimeInMinutes { get; set; }
        public string ProfilePicture { get; set; }
        public VerificationStatus verificationStatus { get; set; }
        public string verificationFileName { get; set; }

        public string Specialization {  get; set; }
        public List<string> SubSpecializations { get; set; }
        public List<ClinicInfoVM>? Clinics { get; set; } = new List<ClinicInfoVM>();

    }
}
