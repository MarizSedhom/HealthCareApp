using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Birth Date")]
        public DateOnly DateOfBirth { get; set; }
        public Gender gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }= new List<Notification>();

        public bool IsDeleted { get; set; }=false;
    }
    public enum Gender
    {
        Male,
        Female
    }
}
