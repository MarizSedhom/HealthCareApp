using HealthCareApp.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Appointment
{
    public class PatientUpcomingAppointmentsVM
    {
        public int Id { get; set; }

        [EnumDataType(typeof(Status))]
        public Status Status { get; set; }
        public DayOfWeek Day { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string DoctorTitle { get; set; }
        public string DoctorName { get; set; }
        public Models.Type Mode { get; set; }
        public string Specialization { get; set; }
        public string Clinic { get; set; }

        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Fees { get; set; }
    }
}
