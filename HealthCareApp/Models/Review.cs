using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsApproved { get; set; }=false;

        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
