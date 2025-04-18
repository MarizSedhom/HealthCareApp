using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public PaymentStatus paymentStatus { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod paymentMethod { get; set; }


        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public virtual Appointment? Appointment { get; set; }

        public bool IsDeleted { get; set; } = false;
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
