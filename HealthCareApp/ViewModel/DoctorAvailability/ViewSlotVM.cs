using HealthCareApp.Models;
using System.ComponentModel;

namespace HealthCareApp.ViewModel.Doctor
{
    public class ViewSlotVM
    {
        public int SlotId { get; set; }

        [DisplayName("Time Range")]
        public string TimeRange { get; set; }
        public TimeOnly? startTime { get; set; }

        [DisplayName("Status")]
        public string Status {  get; set; }
        public bool IsBooked {  get; set; }

        [DisplayName("Patient Name")]
        public string PatientName { get; set; }

        [DisplayName("Patient Number")]
        public string PatientNumber { get; set; }

        public int? AppointmentId {  get; set; }
        public int? AvailabilityId {  get; set; }
        public string?drId { get; set; }
        //public void SetViewSlotVM(AvailabilitySlots s)
        //{
        //    TimeRange = $"{s.StartTime} - {s.EndTime}";
        //    PatientName = (s.Appointment == null) ? "-" : s.Appointment.PatientName;
        //    PatientNumber = (s.Appointment == null) ? "-" : s.Appointment.PatientPhone;
        //    IsBooked = s.IsBooked;
        //    Status = (s.IsBooked) ? "Booked" : "Available";
        //    AppointmentId = (s.Appointment == null) ? null : s.Appointment.patientId;
        //    SlotId = s.patientId;
        //    AvailabilityId = s.AvailabilityId;

        //}
    }
}
