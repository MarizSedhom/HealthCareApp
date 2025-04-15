namespace HealthCareApp.ViewModel.Review
{
    public class ReviewVM
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; } 
        public bool IsEdited { get; set; } 
    }
}
