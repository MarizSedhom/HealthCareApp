using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCare.DAL.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You Must Enter Rate...!")]
        [Range(0, 5, ErrorMessage = "Your Rate Must Be Between 0 and 5 ...!")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "You Must Leave A Review...!")]
        public string ReviewText { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;


        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public virtual Patient? Patient { get; set; }


        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsEdited { get; set; } = false;
    }
}
