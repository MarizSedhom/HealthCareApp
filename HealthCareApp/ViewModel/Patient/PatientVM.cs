using System.ComponentModel.DataAnnotations;
using HealthCareApp.Models;
namespace HealthCareApp.ViewModel.Patient
{
    public class PatientVM
    {
        public string Id { get; set; }

        [Required]
        public int Age { get; set; }

        [Display(Name = "Full Name")]
        [Required]
        [RegularExpression("^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Only letters and single spaces between words are allowed.")]
        public string FullName { get; set; }

        [Display(Name = "Emergency Contact")]
        [Required]
        [RegularExpression(@"^(+?\d{1,3})?[- ]?\d{10}$", ErrorMessage = "Phone number must be 10 digits, with optional country code.")]
        [Unique<Models.Patient>("The Emergency Contact number is already in use.", "EmergencyContact")]
        public string EmergencyContact { get; set; }

        [Display(Name = "Medical History")]
        public string MedicalHistory { get; set; }
    }
}
