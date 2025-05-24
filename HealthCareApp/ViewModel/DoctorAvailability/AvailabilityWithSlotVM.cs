namespace HealthCareApp.ViewModel.Doctor
{
    public class AvailabilityWithSlotVM
    {
        public int AvailabilityId {  get; set; }
        public DateOnly AvailabilityDate { get; set; }
        public HealthCare.DAL.Models.Type AvailabilityType { get; set; }
        public string ClinicName { get; set; }
        public IEnumerable<Slot>Slots { get; set; }

    }
}
