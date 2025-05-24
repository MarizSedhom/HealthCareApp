using HealthCareApp.ViewModel.Doctor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Clinic
{
    public class ClinicCreateVM
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        [DisplayName("Address")]

        public string ClinicAddress { get; set; }
        [DisplayName("Phone Number")]
        [RegularExpression("^01[0125][0-9]{8}$" , ErrorMessage = "Phone must start with 010, 011, 012, or 015 and must be 11 digits long")]
        public string ClinicPhoneNumber { get; set; }
        public Location ClinicLocation { get; set; } = new Location();
        public string? doctorId { get; set; }
        public IEnumerable<Item<int, string>>? cities { get; set; }
        public IEnumerable<Item<int, string>>? regions { get; set; }

        [DisplayName("City")]
        public int SelectedCityId { get; set; }

        [DisplayName("Region")]
        public int SelectedRegionId { get; set; }
        public PaginationInfo? PaginationInfo { get; set; } 
    }
}
