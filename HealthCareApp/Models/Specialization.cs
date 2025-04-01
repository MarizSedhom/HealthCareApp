namespace HealthCareApp.Models
{
    public class Specialization
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SubSpecialization>? SubSpecialization { get; set; }=new List<SubSpecialization>();
        public virtual ICollection<Doctor>? Doctors { get; set; } = new List<Doctor>();

        public bool IsDeleted { get; set; } = false;
    }
}
