using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class SubSpecialization
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Specialization")]
        public int SpecializationId { get; set; }
        public virtual Specialization? Specialization { get; set; }
        public virtual ICollection<Doctor>? Doctors { get; set; } = new List<Doctor>();

        public bool IsDeleted { get; set; } = false;
    }
}
