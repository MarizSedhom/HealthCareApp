using HealthCareApp.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.ViewModel.Doctor
{
    public class GetAvailabilityForDrVM
    {
        public int Id { get; set; }
        [DisplayName("Day")]
        public DayOfWeek dayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        [DisplayName("Time Range")]

        public string? TimeRange { get; set; }
        public DateOnly Date { get; set; }

        [DisplayName("Session Duration")]
        public int Duration { get; set; }
        [DisplayName("Session Type")]

        public Models.Type type { get; set; }
        [DisplayName("Available Slots")]
        public int AvailableSlotsCnt { get; set; }

        [DisplayName("Appointments")]

        public int AppointmentCnt { get; set; }

        public string DoctorId { get; set; }
        [DisplayName("Clinic Name")]
        public String ClinicName { get; set; }

        //void SettingsBindableAttribute() { }
    }
}
