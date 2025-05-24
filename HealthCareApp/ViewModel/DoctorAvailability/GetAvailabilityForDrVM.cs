﻿using System.ComponentModel;
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

        [DisplayName("Hours")]

        public string? TimeRange { get; set; }
        public DateOnly Date { get; set; }

        [DisplayName("Session")]
        public int Duration { get; set; }
        [DisplayName("Type")]

        public HealthCare.DAL.Models.Type type { get; set; }
        [DisplayName("Slots")]
        public int AvailableSlotsCnt { get; set; }

        [DisplayName("Appointments")]

        public int AppointmentCnt { get; set; }

        public string DoctorId { get; set; }
        [DisplayName("Clinic Name")]
        public String ClinicName { get; set; }

        //void SettingsBindableAttribute() { }
    }
}
