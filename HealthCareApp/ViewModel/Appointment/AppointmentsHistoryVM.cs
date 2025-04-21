using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Appointment
{
    public class AppointmentsHistoryVM
    {
        [Key]
        public int Id { get; set; }
        public Status Status { get; set; }
        public string Date { get; set; }

        [Display(Name = "Time")]
        public TimeOnly StartTime { get; set; }

        [Display(Name = "Doctor")]
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public Models.Type Mode { get; set; }
        public string Clinic { get; set; }

        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; }
    }
}
