using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCare.DAL.Models
{
    public class AvailabilitySlots
    {
        public int Id { get; set; }

        [Display(Name = "Slot")]
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBooked { get; set; } = false;
        public virtual Appointment? Appointment { get; set; }

        [ForeignKey("Availability")]
        public int AvailabilityId { get; set; }
        public virtual Availability? Availability { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
