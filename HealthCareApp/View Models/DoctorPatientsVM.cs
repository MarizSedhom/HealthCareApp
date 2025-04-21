namespace HealthCareApp.View_Models
{
    public class DoctorPatientsVM
    {
        public virtual List<PatientVM>? Patients { get; set; }
        public bool IsFirst { get; set; }
    }
}
