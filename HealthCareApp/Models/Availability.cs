using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Availability
    {
        public int Id { get; set; }

        [Display(Name = "Day")]
        public DayOfWeek dayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly Date { get; set; }
        public int Duration { get; set; }
        public Type type { get; set; }

        public virtual ICollection<AvailabilitySlots>? AvailableSlots { get; set; } = new List<AvailabilitySlots>();

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        [ForeignKey("Clinic")]
        public int ClinicId { get; set; }
        public virtual Clinic? Clinic { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
    public enum Type
    {
        Online,
        Offline
    }
}
