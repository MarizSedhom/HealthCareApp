using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCare.DAL.Models
{
    public class Region
    {
        public int Id { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public string RegionNameAr { get; set; }
        public string RegionNameEn { get; set; }

        public City City { get; set; }
    }
}
