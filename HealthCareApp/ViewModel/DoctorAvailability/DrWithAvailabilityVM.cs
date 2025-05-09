using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DrWithAvailabilityVM
    {
        public GetAvailabilityForDrVM GetAvailabilityForDrVM { get; set; }
        public string DrId { get; set; }
        [DisplayName("Specialization Name")]

        public string SpecializationName { get; set; }
        public string name {  get; set; }
    }
}
