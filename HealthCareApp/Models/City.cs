namespace HealthCareApp.Models
{
    public class City
    {
        public int Id { get; set; }
        public string CityNameAr { get; set; }
        public string CityNameEn { get; set; }
        public virtual ICollection<Region> ?Regions { get; set; } = new List<Region>();
    }
}
