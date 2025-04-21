using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Patient
{
    public class EditPatientInfoVM
    {
        [Key]
        public string patientId { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "First Name")]
        public string LastName { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; }

        [Display(Name = "Medical History")]
        public string MedicalHistory { get; set; }
    }
}
