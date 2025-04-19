using Microsoft.EntityFrameworkCore;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DoctorInfoVM
    {
        public string DoctorId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; }
        public decimal Fees { get; set; }
        public Gender gender { get; set; }
        public int WaitingTimeInMinutes { get; set; }
        public int ExperienceYears { get; set; }
        public string ProfilePicture { get; set; }
        public List<string> SubSpecializations { get; set; }
        public List<ClinicInfoVM>? Clinics { get; set; } = new List<ClinicInfoVM>();
    }
}
