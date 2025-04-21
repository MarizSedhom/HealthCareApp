using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class SubSpecialization
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    [RegularExpression(@"^[A-Za-z&,]+(?: [A-Za-z&,]+)*(?: \([A-Za-z&,]+(?: [A-Za-z&,]+)*\))?$",
        ErrorMessage = "Invalid format. Must be words with single spaces and an optional description in parentheses.")]
    [Unique<SubSpecialization>("The name is already in use.", "Name")]
    //[Unique<SubSpecialization>("Combination of subspecialization name & specialization name is already in use.", "SpecializationId", "patientId")]
    public string Name { get; set; }

    [ForeignKey("Specialization")]
    [Required]
    [GreaterThanZero<SubSpecialization>("SpecializationId", "Specialization field is Requied")]
    public int SpecializationId { get; set; }

    public virtual Specialization? Specialization { get; set; }
    public virtual ICollection<Doctor>? Doctors { get; set; } = new List<Doctor>();
    public bool IsDeleted { get; set; } = false;
}
