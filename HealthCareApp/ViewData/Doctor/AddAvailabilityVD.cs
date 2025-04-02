namespace HealthCareApp.ViewData.Doctor
{
    public class AddAvailabilityVD
    {
        public ICollection<DrAvailabilityVD> DrAvailability { get; set; };
        public ICollection<Item<int>> DrClincs;
    }
}
