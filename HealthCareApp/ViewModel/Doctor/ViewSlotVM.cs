using HealthCareApp.Models;
using System.ComponentModel;

namespace HealthCareApp.ViewModel.Doctor
{
    public class ViewSlotVM
    {
        public int SlotId { get; set; }

        [DisplayName("Time Range")]
        public string TimeRange { get; set; }
        [DisplayName("Patient Name")]
        public string  PatientName { get; set; }
        [DisplayName("Status")]
        public bool IsBooked {  get; set; }
    }
}
