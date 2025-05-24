using System.ComponentModel.DataAnnotations;

namespace HealthCare.DAL.Models
{
    public class Specialization
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [RegularExpression(@"^[A-Za-z&,]+(?: [A-Za-z&,]+)*(?: \([A-Za-z&,]+(?: [A-Za-z&,]+)*\))?$",
        ErrorMessage = "Invalid format. Must be words with single spaces and an optional description in parentheses.")]
        //[Unique<Specialization>("The name is already in use.", "Name")]
        public string Name { get; set; }

        public virtual ICollection<SubSpecialization>? SubSpecialization { get; set; } = new List<SubSpecialization>();
        public virtual ICollection<Doctor>? Doctors { get; set; } = new List<Doctor>();

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
