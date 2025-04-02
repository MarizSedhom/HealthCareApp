using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public Status status { get; set; }
        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey("AvailableSlot")]
        public int SlotId { get; set; }
        public virtual AvailabilitySlots? AvailableSlot { get; set; }

        public PaymentStatus paymentStatus { get; set; } = PaymentStatus.Pending;// default pending
        public decimal Amount { get; set; } // doctor fees
        public PaymentMethod paymentMethod { get; set; }

        //[ForeignKey("Payment")]
        //public int PaymentId { get; set; }
        //public virtual Payment? Payment { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    public enum Status
    {
        Pending,
        Completed,
        Cancelled
    }
    public enum PaymentStatus
    {
        Pending,
        Failed,
        Paid
    }
    public enum PaymentMethod
    {
        Cash,
        Visa
    }
}
