using HealthCareApp.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DrUpdateProfileVM
    {
        [DisplayName("First Name")]
        [RegularExpression(@"^[A-Z][a-zA-Z'-]{1,29}$",ErrorMessage ="must be letter")]
        public string FirstName { get; set; }


        [DisplayName("Last Name")]
        [RegularExpression(@"^[A-Z][a-zA-Z'-]{1,29}$", ErrorMessage = "must be letter")]
        public string LastName { get; set; }

        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]

        public DateOnly DateOfBirth { get; set; }

        [DisplayName("Gender")]
        public Gender gender { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Currency,ErrorMessage ="Fees must be Number")]
        
        public decimal Fees { get; set; }

        [DisplayName("Experience Years")]
        public int ExperienceYears { get; set; }

        [DisplayName("Waiting Time (min)")]
        public int WaitingTimeInMinutes { get; set; }

        [DisplayName("Profile Picture")]
        public string? ProfilePicture { get; set; }
        public IEnumerable<string> Clinics {  get; set; }
        public string Specialization {  get; set; }  //admin only can change that
        public IEnumerable<string>? SubSpecializations { get; set; }  //admin only can change that

    }
}
