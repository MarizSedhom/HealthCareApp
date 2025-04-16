using HealthCareApp.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Appointment
{
    public class UpcomingAppointmentsVM
    {
        public DayOfWeek Day { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string PatientId { get; set; }

        [Display(Name = "Patient Name")]
        public string PatientName { get; set; }

        [Display(Name = "Patient Phone")]
        public string PatientPhone { get; set; }
        public Models.Type Mode { get; set; }

        [Display(Name = "Payment Status")]
        public PaymentStatus paymentStatus { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethod paymentMethod { get; set; }

    }
}
