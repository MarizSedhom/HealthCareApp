using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClinicAddress { get; set; }
        public string ClinicCity { get; set; }
        public string ClinicRegion { get; set; }
        public string ClinicPhoneNumber { get; set; }


        public virtual ICollection<Availability>? Availabilities { get; set; }=new List<Availability>();

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        public bool IsDeleted { get; set; } = false;



    }
}
