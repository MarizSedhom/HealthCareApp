using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [EnumDataType(typeof (Status))]
        public Status Status { get; set; } = Status.Upcoming;

        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public virtual Patient? Patient { get; set; }


        [ForeignKey("AvailableSlot")]
        public int SlotId { get; set; }
        public virtual AvailabilitySlots? AvailableSlot { get; set; }


        [EnumDataType(typeof(PaymentStatus))]
        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public decimal Amount { get; set; }

        [Required]
        [EnumDataType(typeof(PaymentMethod))]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public enum Status
    {
        Upcoming,
        Completed,
        Cancelled,
        Rescheduled
    }
    public enum PaymentStatus
    {
        Unpaid,
        Failed,
        Paid,
        Refunded
    }
    public enum PaymentMethod
    {
        Cash,
        Visa
    }
}
