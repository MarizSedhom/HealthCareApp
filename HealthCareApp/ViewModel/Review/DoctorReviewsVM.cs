namespace HealthCareApp.ViewModel.Review
{
    public class DoctorReviewsVM
    {
        public double TotalRating { get; set; }
        public int ReviewsCount { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public IEnumerable<ReviewVM> Reviews { get; set; }
    }
}
