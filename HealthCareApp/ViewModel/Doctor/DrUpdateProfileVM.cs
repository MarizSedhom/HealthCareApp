using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DrUpdateProfileVM
    {
        public string DrId {  get; set; }
        [DisplayName("First Name")]
        [RegularExpression(@"^[a-zA-Z]{1,29}$", ErrorMessage = "First name must contain only English letters and be up to 29 characters.")]
        public string FirstName { get; set; }


        [DisplayName("Last Name")]
        [RegularExpression(@"^[a-zA-Z]{1,29}$", ErrorMessage = "First name must contain only English letters and be up to 29 characters.")]
        public string LastName { get; set; }


        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [DisplayName("Gender")]
        public Gender gender { get; set; }
        public Title? Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Currency,ErrorMessage ="Fees must be Number")]
        [Range(1, double.MaxValue, ErrorMessage = "Fees should be greater than Zero")]

        public decimal Fees { get; set; }

        [DisplayName("Experience Years")]
        [Range(0, double.MaxValue, ErrorMessage = "Experience Years shouldn't be Negative value")]
        public int ExperienceYears { get; set; }

        [DisplayName("Waiting Time (min)")]
        [Range(0, double.MaxValue, ErrorMessage = "Waiting Time shouldn't be Negative value")]
        public int WaitingTimeInMinutes { get; set; }

        [DisplayName("Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
        public string? CurrentPicturePath { get; set; }

        public string ?ImgName {  get; set; }
        public string? CurrentVerficationFilePath { get; set; }
        public string? VerficationFileName { get; set; }

        public IEnumerable<string> ?Clinics {  get; set; }
        public string ?Specialization {  get; set; }  //admin only can change that
        public IEnumerable<string>? SubSpecializations { get; set; }  //admin only can change that


        //[Display(Name = "Phone Number")]
        //[RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Must be exactly 12 digits")]
        //[DataType(DataType.PhoneNumber)]
        //public string PhoneNumber { get; set; }

        //public IEnumerable<Title>titles { get; set; }
    }
}
