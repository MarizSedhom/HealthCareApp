using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Review
{
    public class AddReviewVM
    {
        public string DoctorId { get; set; }

        [Required(ErrorMessage = "You Must Enter Rate...!")]
        [Range(0, 5, ErrorMessage = "Your Rate Must Be Between 0 and 5 ...!")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "You Must Leave A Review...!")]
        public string ReviewText { get; set; }
    }
}
