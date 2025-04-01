using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class AvailabilitySlots
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBooked { get; set; }=false;

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public virtual Appointment? Appointment { get; set; }

        [ForeignKey("Availability")]
        public int AvailabilityId { get; set; }
        public virtual Availability? Availability { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
