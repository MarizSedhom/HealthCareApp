using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Appointment
{
    public class UpcomingAppointmentsVM
    {
        [EnumDataType(typeof(Status))]
        public Status Status { get; set; } 
        public DayOfWeek Day { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string PatientId { get; set; }

        [Display(Name = "Patient Name")]
        public string PatientName { get; set; }

        [Display(Name = "Patient Phone")]
        public string PatientPhone { get; set; }
        public HealthCare.DAL.Models.Type Mode { get; set; }

        [Display(Name = "Payment Status")]
        public PaymentStatus paymentStatus { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethod paymentMethod { get; set; }

    }
}