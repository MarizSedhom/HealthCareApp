using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Patient
{
    public class PatientsInfoVM
    {
        [Key]
        public string Id { get; set; }
        public int Age { get; set; }

        [Display(Name = "Patient Name")]
        public string PatientName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; }

        [Display(Name = "Medical History")]
        public string MedicalHistory { get; set; }
    }
}
