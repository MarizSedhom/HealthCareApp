using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }


        [EnumDataType(typeof (Status))]
        public Status Status { get; set; } = Status.Pending;


        [Required(ErrorMessage = "You Must Enter Patient Name...!!")]
        [RegularExpression(@"^\w+\s\w+$", ErrorMessage = "You Must Enter Full Name [First, Last]")]
        [Display(Name ="Patient Name")]
        public string PatientName { set; get; }


        [Required(ErrorMessage = "You Must Enter Phone Number...!!")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(11, MinimumLength = 11 ,ErrorMessage = "Phone Number Must Be Exactly 11 Digits.")]
        [Display(Name = "Patient Phone")]
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
        [Display(Name = "Payment Method")]
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
