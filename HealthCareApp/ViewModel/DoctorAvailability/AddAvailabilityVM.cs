namespace HealthCareApp.ViewModel.Doctor
{
    public class AddAvailabilityVM
    {
        public string id { get; set; }
        public List<DrAvailabilityVM> DrAvailability { get; set; }
        public ICollection<Item<int, string>> DrClincs { get; set; } = new List<Item<int, string>>();
    }
}
