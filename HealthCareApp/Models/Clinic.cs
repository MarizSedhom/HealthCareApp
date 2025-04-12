using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        [RegularExpression("^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Only letters and single spaces between words are allowed.")]
        [Unique<Clinic>("The name is already in use.", "Name")]
        public string Name { get; set; }

        [Display(Name = "Address")]
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Only letters and single spaces between words are allowed.")]
        public string ClinicAddress { get; set; }

        [Display(Name = "City")]
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Only letters and single spaces between words are allowed.")]
        public string ClinicCity { get; set; }

        [Display(Name = "Region")]
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Only letters and single spaces between words are allowed.")]
        public string ClinicRegion { get; set; }

        [Display(Name = "Phone Number")]
        [Required]
        [MaxLength(20)]
        [RegularExpression(@"^(\+?\d{1,3})?[- ]?\d{10}$", ErrorMessage = "Phone number must be 10 digits, with optional country code.")]
        public string ClinicPhoneNumber { get; set; }


        public virtual ICollection<Availability>? Availabilities { get; set; }=new List<Availability>();

        [ForeignKey("Doctor")]
        public string? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
