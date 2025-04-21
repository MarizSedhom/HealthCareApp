using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HealthCareApp.ViewModel.Doctor
{
    public class AdminUpdateDrVM
    {
            public string doctorId { get; set; }
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

            [DataType(DataType.Currency, ErrorMessage = "Fees must be Number")]
            public decimal Fees { get; set; }

            [DisplayName("Experience Years")]
            public int ExperienceYears { get; set; }

            [DisplayName("Waiting Time (min)")]
            public int WaitingTimeInMinutes { get; set; }

            [DisplayName("Profile Picture")]
            public IFormFile? ProfilePicture { get; set; }
            public string? CurrentPicturePath { get; set; }
            public string? ImgName { get; set; }
            public string? verificationFileName { get; set; }
        //public IFormFile? ImagFromDr { get; set; }

            [DisplayName("verification File")]
            public IFormFile? verificationFileFromDr { get; set; }
            public string? CurrrentverificationPath { get; set; }
            public IEnumerable<string>? Clinics { get; set; }

         
            [DisplayName("Specialization ")]
            public int SelectedSpecialization { get; set; }  //admin only can change that
            public IEnumerable<Item<int, string>> ?Specializations { get; set; } = new List<Item<int, string>>();
            [DisplayName("Sub Specializations")]
            public IEnumerable<int> SelectedSubSpecializations { get; set; }  //admin only can change that
            public IEnumerable<Item<int, string>>? SubSpecializationsList { get; set; } = new List<Item<int, string>>();


            public AdminUpdateDrVM()
            {
                
            }
            public AdminUpdateDrVM(Models.Doctor doctor)
            {
                SetDataFromDoctor(doctor);
            }
            public void SetDataFromDoctor(Models.Doctor doctor)
            {
                doctorId = doctor.Id;
                FirstName = doctor.FirstName;
                LastName = doctor.LastName;
                DateOfBirth = doctor.DateOfBirth;
                gender = doctor.gender;
                Title = doctor.Title;
                Description = doctor.Description;
                Fees = doctor.Fees;
                ExperienceYears=doctor.ExperienceYears;
                WaitingTimeInMinutes = doctor.WaitingTimeInMinutes;
                ImgName = doctor.ProfilePicture;
                verificationFileName = doctor.verificationFileName;
                SelectedSpecialization = doctor.Specialization.Id;
                SelectedSubSpecializations = doctor.SubSpecializations.Select(x => x.Id);
            }


    }
}
