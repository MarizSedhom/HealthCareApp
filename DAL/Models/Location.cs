using System.ComponentModel.DataAnnotations;

namespace HealthCare.DAL.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string? KeyId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
