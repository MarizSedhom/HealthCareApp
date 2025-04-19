using HealthCareApp.ViewModel.Doctor;

namespace HealthCareApp.ViewModel.Clinic
{
    public class ClinicCreateVM
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ClinicAddress { get; set; }
        public string ClinicPhoneNumber { get; set; }
        public string doctorId { get; set; }
        public IEnumerable<Item<int, string>>? cities { get; set; }
        public IEnumerable<Item<int, string>>? regions { get; set; }

        public int SelectedCityId { get; set; }
        public int SelectedRegionId { get; set; }
        public PaginationInfo? PaginationInfo { get; set; } 
    }
}
