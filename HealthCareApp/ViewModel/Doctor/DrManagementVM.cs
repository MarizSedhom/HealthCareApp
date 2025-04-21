using HealthCareApp.ViewModel.Appointment;
using HealthCareApp.ViewModel.Clinic;
using System;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DrManagementVM
    {
        public AdminUpdateDrVM DoctorVM { get; set; }
        public IEnumerable< ClinicInfoVM >clinicInfoVMs { get; set; }
        public IEnumerable< GetAvailabilityForDrVM>? Availabilities {  get; set; }
        public IEnumerable< UpcomingAppointmentsVM>  upcomingAppointmentsVM { get; set; }
        public string? doctorId { get; set; }
        public string? drInf {  get; set; }
        public string? clinics {  get; set; }

        public string  schedule {  get;set; }
        public string patients {  get; set; }

    }
}
