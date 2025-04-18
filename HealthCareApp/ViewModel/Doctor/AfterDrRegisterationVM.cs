using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Doctor
{
    public class AfterDrRegisterationVM
    {
        [Required(ErrorMessage = "Profile picture is required")]
        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture {  get; set; }
        //public string PictureReativeUrl { get; set; }
        public string? DoctorId {  get; set; }
        public string? Specialization { get; set; }
        public IEnumerable< Item<int ,string>>? SubSpecialization { get; set; }
        [DisplayName("Sub Specialization")]
        public IEnumerable<int>  SelectedSubSpecializations { get; set; }

        [DisplayName("Medical Credentials")]
        public IFormFile doctorVerificationFile { get; set; }
        [DisplayName("Fees")]
        public decimal Fees { get; set; }
        [DisplayName("Waiting Time (Min)")]
        public int WaitingTimeInMinutes { get; set; }


    }
}
