using HealthCareApp.ViewModel.Doctor;

namespace HealthCareApp.ViewModel.Clinic
{
    public class ClinicInfoVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClinicAddress { get; set; }
        public string ClinicCity { get; set; }
        public string ClinicRegion { get; set; }
        public string ClinicPhoneNumber { get; set; }
        public Location ClinicLocation { get; set; } = new Location();
        public string? doctorId { get; set; }
        public PaginationInfo? PaginationInfo { get; set; }

    }
}
