using Microsoft.EntityFrameworkCore;

namespace HealthCareApp.ViewModel.Patient
{
    public class PatientInfoForDoctorVM
    {
        public string PatientFullName { get; set; }
        public int Age { get; set; }
        public string MedicalHistory { get; set; }

    }
}
