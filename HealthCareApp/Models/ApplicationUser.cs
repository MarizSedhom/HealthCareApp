using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HealthCareApp.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Display(Name = "Name")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
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




