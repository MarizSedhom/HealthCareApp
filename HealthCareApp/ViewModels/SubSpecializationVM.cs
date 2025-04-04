using HealthCareApp.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModels
{
    public class SubSpecializationVM
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z&,\s]+(?:\s\([a-zA-Z&,\s]+\))?$", ErrorMessage = "Invalid format. The string must contain only letters, ampersands, commas, spaces, and optionally, a description in parentheses.")]
        [Unique<SubSpecialization>("Name", ErrorMessage = "The name is already in use.")]
        public string Name { get; set; }

        public int SpecializationId { get; set; }
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Specialization Name")]
        public string SpecializationName { get; set; } // for ui only
        public IEnumerable<Specialization>? Specializations { get; set; } // for ui only
    }
}
