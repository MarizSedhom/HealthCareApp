using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Appointment
{
    public class ReserveAppointmentVM
    {
        public int SlotId { get; set; }
        public DayOfWeek Day { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public string? DoctorId { get; set; }
        public string? DoctorImg { get; set; }
        public string? DoctorTitle { get; set; }
        public string? DoctorName { get; set; }
        public int? WaitingTime { get; set; }

        [Display(Name = "Patient Name")]
        public string? PatientName { get; set; }

        [Display(Name = "Patient Phone")]
        public string? PatientPhone { get; set; }
        public HealthCare.DAL.Models.Type Mode { get; set; }
        public string? Specialization { get; set; }
        public string? DoctorDescription { get; set; }
        public string? Clinic { get; set; }

        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Fees { get; set; }
    }
}
