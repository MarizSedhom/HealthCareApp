using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Patient
{
    public class PatientStatVM
    {
        [Key]
        public string PatientId { get; set; }
        public int CompletedAppointments { get; set; }
        public int CanceledAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int RescheduledAppointments { get; set; }
        public int ApprovedReviews { get; set; }
        public int RejectedReviews { get; set; }
        public int PendingReviews { get; set; }
    }
}
