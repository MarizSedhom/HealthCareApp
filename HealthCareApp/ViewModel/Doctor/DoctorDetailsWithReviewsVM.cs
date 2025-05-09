using HealthCareApp.ViewModel.Review;

namespace HealthCareApp.ViewModel.Doctor
{
    public class DoctorDetailsWithReviewsVM
    {
        public HealthCare.DAL.Models.Doctor Doctor { get; set; }
        public List<ReviewVM> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
    }
}
