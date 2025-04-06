using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Appointment
    {
        [Required]
        public int Id { get; set; }

        [EnumDataType(typeof (PaymentStatus))]
        public Status status { get; set; } = Status.Pending;

        [Required]
        [RegularExpression(@"^\w+\s\w+$")]
        public string PatientName { set; get; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PatientPhone { set; get; }


        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public virtual Patient? Patient { get; set; }


        [ForeignKey("AvailableSlot")]
        public int SlotId { get; set; }
        public virtual AvailabilitySlots? AvailableSlot { get; set; }


        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus paymentStatus { get; set; } = PaymentStatus.Pending;

        public decimal Amount { get; set; }

        [Required]
        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod paymentMethod { get; set; }
    }

    public enum Status
    {
        Pending,
        Completed,
        Cancelled // ?? won't mark it as cancelled because it'll be auto deleted
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
