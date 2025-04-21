namespace HealthCareApp.ViewModel.Admin
{
    public class DashboardVM
    {

        public int TotalPatients { get; set; }
        public int TotalPannedPatients { get; set; }
        public int TotalMalePatients { get; set; }
        public int TotalFemalePatients { get; set; }
        public int AvgPatientsAge { get; set; }
        public int AvgMalePatientsAge { get; set; }
        public int AvgFemalePatientsAge { get; set; }
        public int TotalMaleDoctors { get; set; }
        public int TotalFemaleDoctors { get; set; }
        public int AvgDoctorsAge { get; set; }
        public int AvgMaleDoctorsAge { get; set; }
        public int AvgFemaleDoctorsAge { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalApprovedDoctors { get; set; }
        public int TotalRejectedDoctors { get; set; }
        public int TotalProfessors { get; set; }
        public int TotalLecturers { get; set; }
        public int TotalConsultants { get; set; }
        public int TotalSpecialists{ get; set; }
        public int TotalUpcomingAppointments { get; set; }
        public int TotalRescheduledAppointments { get; set; }
        public int TotalCancelledAppointments { get; set; }
        public int TotalCompletedAppointments { get; set; }
        public int TotalPayments { get; set; }
        public int TotalPendingReviews { get; set; }
        public int TotalApprovedReviews { get; set; }
        public int TotalRejectedReviews { get; set; }
    }
}
